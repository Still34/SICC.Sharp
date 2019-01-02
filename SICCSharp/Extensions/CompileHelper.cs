using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Context;
using SICCSharp.Entities;

namespace SICCSharp.Extensions
{
    public static class CompileHelper
    {
        /// <summary>
        ///     Gets the unicode representation of the tab character.
        /// </summary>
        private const char TabChar = '\u0009';

        /// <summary>
        ///     Gets the pre-defined delimiters used to split instruction entries within the line.
        /// </summary>
        private static char[] Delimiters
            => new[] {TabChar, ' '};

        /// <summary>
        ///     Gets the method name that called this method. Used for Serilog logging.
        /// </summary>
        private static string GetMethodName([CallerMemberName] string name = null)
            => name;

        /// <summary>
        ///     Starts compilation process for SIC ASM files.
        /// </summary>
        /// <param name="input">The input filepath. Must include absolute full path including filename.</param>
        /// <param name="output">The output filepath. Must be an absolute path.</param>
        public static async Task CompileAsync(string input, string output)
        {
            using (var inputStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, 128,
                FileOptions.Asynchronous | FileOptions.RandomAccess))
            using (var inputFile = new StreamReader(inputStream))
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                string line;
                var lineCounter = 1;
                string baseMemory = null;
                var memoryBufferHex = -1;
                var locationBufferHex = -1;
                var instructionList = new List<Instruction>();
                // First pass
                while (!string.IsNullOrEmpty(line = await inputFile.ReadLineAsync()))
                {
                    // check if base memory was found and set
                    if (baseMemory != null)
                        if (memoryBufferHex != -1)
                            baseMemory = (memoryBufferHex + locationBufferHex).ToString("X");

                    // split current line into label, instruction, parameter (where possible)
                    var instruction = ParseInstructions(line);

                    // memory location calculation
                    if (instruction.Mnemonic.Name == MnemonicList.StartInstruction.Name)
                        baseMemory = instruction.MemoryLocation;
                    // check if baseMemory is set; otherwise assume 0
                    if (baseMemory == null)
                    {
                        baseMemory = "0";
                        Log.Information("Base memory assignment was not found, assuming 0...");
                    }
                    // if baseMemory is set, get current line's memory and location increment as buffer for next line to add
                    else
                    {
                        var locationOffset = MathHelper.CalculateLocationOffset(instruction);
                        memoryBufferHex = int.Parse(baseMemory, NumberStyles.HexNumber);
                        locationBufferHex = int.Parse(locationOffset, NumberStyles.HexNumber);
                        instruction.MemoryLocation = baseMemory;
                    }

                    instruction.LineCount = lineCounter;
                    instructionList.Add(instruction);
                    lineCounter++;
                }

                // print SYMTAB
                Log.Verbose("SYMTAB (Symbol Table)");
                var symTab = GetSymbolInstructions(instructionList);
                foreach (var instruction in symTab)
                    Log.Verbose("{MemoryLocation} - {Label} - {Parameter}", instruction.MemoryLocation,
                        instruction.Label, instruction.Parameter);

                // Second pass
                // compile obj code
                foreach (var instruction in instructionList)
                {
                    // ignore if it's one of the whitelisted instructions
                    if (MnemonicList.BeginEndInstructions.Any(x => x.Name == instruction.Mnemonic.Name)) continue;
                    var instructionParameter = (string) instruction.Parameter;
                    var splitParameter = instructionParameter?.Split(',');
                    // indexed mode
                    if (splitParameter?.Length > 1)
                    {
                        var targetVariable = splitParameter[0];
                        var variableInstruction = symTab.FirstOrDefault(x => x.Label == targetVariable);
                        if (variableInstruction == null)
                        {
                            Log.Warning(
                                "Cannot find equivalent variable in SYMTAB for instruction {Name} on line {LineCount}",
                                instruction.Mnemonic.Name, instruction.LineCount);
                        }
                        else
                        {
                            var charArray = MathHelper.CalculateOpCodeBitArray(variableInstruction.MemoryLocation);
                            charArray[0] = '1';
                            var charArrayString = new string(charArray);
                            var targetAddress = Convert.ToInt32(charArrayString, 2).ToString("X");
                            instruction.OpCode = instruction.Mnemonic.OpCode + targetAddress;
                        }
                    }
                    // direct mode
                    else
                    {
                        Instruction targetInstruction = null;
                        if (instructionParameter != null)
                        {
                            targetInstruction = instructionList.FirstOrDefault(x => x.Label == instructionParameter);
                            if (instruction.Mnemonic.Name == "WORD")
                            {
                                instruction.OpCode = FormatHexToLength(instructionParameter, 6);
                                continue;
                            }

                            if (targetInstruction == null)
                            {
                                Log.Verbose(
                                    "Cannot find equivalent variable in SYMTAB for instruction {Name} on line {LineCount}",
                                    instruction.Mnemonic.Name, instruction.LineCount);
                                continue;
                            }
                        }

                        instruction.OpCode =
                            (instruction.Mnemonic.OpCode + targetInstruction?.MemoryLocation).PadRight(6, '0');
                    }
                }

                // print overall
                var summaryBuilder = new StringBuilder();
                foreach (var instruction in instructionList)
                {
                    // check if it's an instruction such as START or END
                    // if yes, we don't pop in the memory location string
                    if (MnemonicList.BeginEndInstructions.All(x => x.Name != instruction.Mnemonic.Name))
                        summaryBuilder.Append(instruction.MemoryLocation);

                    summaryBuilder.Append(TabChar);
                    if (!string.IsNullOrEmpty(instruction.Label)) summaryBuilder.Append(instruction.Label);

                    summaryBuilder.Append(TabChar);
                    if (!string.IsNullOrEmpty(instruction.Mnemonic.Name)) summaryBuilder.Append(instruction.Mnemonic);

                    summaryBuilder.Append(TabChar);
                    if (!string.IsNullOrEmpty((string) instruction.Parameter))
                        summaryBuilder.Append(instruction.Parameter);

                    summaryBuilder.Append(TabChar);
                    if (!string.IsNullOrEmpty(instruction.OpCode)) summaryBuilder.Append(instruction.OpCode);

                    summaryBuilder.AppendLine();
                }

                // get target file name for outputting the files
                var targetFileName = Path.GetFileNameWithoutExtension(input);

                // write ASM output inc. object code and location table
                Log.Information(summaryBuilder.ToString());
                var asmOutput = Path.Combine(output, $"{targetFileName}.txt");
                await File.WriteAllTextAsync(asmOutput, summaryBuilder.ToString());
                Log.Information("Saved ASM output to {asmOutput}.", asmOutput);

                // compile object program
                var objectProgram = CompileObjectProgram(instructionList);
                Log.Information(objectProgram);
                var objectProgramOutput = Path.Combine(output, $"{targetFileName}.obj");
                await File.WriteAllTextAsync(objectProgramOutput, objectProgram);
                Log.Information("Saved object program output to {objectProgramOutput}.", objectProgramOutput);
            }
        }

        /// <summary>
        ///     Compiles the complete object program with given instructions.
        /// </summary>
        public static string CompileObjectProgram(IEnumerable<Instruction> instructions)
        {
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                // conditional checks
                if (instructions == null) throw new ArgumentNullException(nameof(instructions));
                var orderedInstructions = instructions.OrderBy(x => x.LineCount);
                if (!orderedInstructions.Any())
                    throw new ArgumentOutOfRangeException(nameof(instructions),
                        "Compiler error: instruction size zero.");
                if (orderedInstructions.All(x => string.IsNullOrEmpty(x.MemoryLocation)))
                    throw new InvalidOperationException(
                        "Compiler error: an attempt was made to compile the object program, but none of the instructions contain a valid memory address.");

                // init builder
                var objectProgramBuilder = new StringBuilder();

                // get header name; otherwise default to PROG
                var firstInstruction =
                    orderedInstructions.FirstOrDefault(x => x.Mnemonic.Name == MnemonicList.StartInstruction.Name) ??
                    orderedInstructions.FirstOrDefault();
                var lastInstruction =
                    orderedInstructions.LastOrDefault(x => x.Mnemonic.Name == MnemonicList.EndInstruction.Name) ??
                    orderedInstructions.LastOrDefault();
                objectProgramBuilder.Append(BuildHeaderRecord(firstInstruction, lastInstruction));

                var commandInstructions = new List<Instruction>();
                var symbolInstructions = new List<Instruction>();
                foreach (var instruction in orderedInstructions)
                    if (MnemonicList.StandardInstructions.Any(x => x.OpCode == instruction.Mnemonic.OpCode) &&
                        MnemonicList.BeginEndInstructions.All(x => x.Name != instruction.Mnemonic.Name))
                        commandInstructions.Add(instruction);
                    else if (MnemonicList.SymbolInstructions.Any(x => x.Name == instruction.Mnemonic.Name))
                        symbolInstructions.Add(instruction);

                objectProgramBuilder.Append(BuildTextRecord(commandInstructions));
                objectProgramBuilder.Append(BuildTextRecord(symbolInstructions));

                objectProgramBuilder.AppendLine();
                objectProgramBuilder.AppendLine(BuildEndRecord(orderedInstructions));

                return objectProgramBuilder.ToString();
            }
        }

        /// <summary>
        ///     Builds the header record for an object program.
        /// </summary>
        public static string BuildHeaderRecord(Instruction startInstruction, Instruction endInstruction)
        {
            if (startInstruction.Mnemonic.Name != MnemonicList.StartInstruction.Name)
                throw new ArgumentException("Start instruction given in head record builder is not valid.",
                    nameof(startInstruction));
            if (endInstruction.Mnemonic.Name != MnemonicList.EndInstruction.Name)
                throw new ArgumentException("End instruction given in head record builder is not valid.",
                    nameof(endInstruction));

            var recordBuilder = new StringBuilder("H^");
            // fallback to default header name if instruction label does not exist
            recordBuilder.Append(string.IsNullOrEmpty(startInstruction.Label) ? "PROG" : startInstruction.Label);
            recordBuilder.Append('^');
            recordBuilder.Append(FormatHexToLength(startInstruction.MemoryLocation, 6));
            recordBuilder.Append('^');
            var programSize = MathHelper.SubHex(endInstruction.MemoryLocation, startInstruction.MemoryLocation);
            recordBuilder.Append(FormatHexToLength(programSize, 6));

            return recordBuilder.ToString();
        }

        /// <summary>
        ///     Builds the text record for an object program.
        /// </summary>
        public static string BuildTextRecord(IEnumerable<Instruction> instructions)
        {
            var bufferBuilder = new StringBuilder();
            foreach (var chunkedInstructions in instructions.Chunk(8))
            {
                var instructionCount = 0;
                // ToList to avoid multiple enumeration
                var chunkedInstructionsList =
                    chunkedInstructions.Where(x => !string.IsNullOrWhiteSpace(x.OpCode)).ToList();
                var chunkedInstructionsLength =
                    string.Join("", chunkedInstructionsList.Select(x => x.OpCode)).Length / 2;
                foreach (var chunkedInstruction in chunkedInstructionsList)
                {
                    if (instructionCount == 0)
                    {
                        bufferBuilder.AppendLine();
                        bufferBuilder.Append("T^");
                        bufferBuilder.Append(FormatHexToLength(chunkedInstruction.MemoryLocation, 6));
                        bufferBuilder.Append('^');
                        bufferBuilder.Append(FormatHexToLength(chunkedInstructionsLength.ToString("X"), 2));
                        bufferBuilder.Append('^');
                    }

                    bufferBuilder.Append(FormatHexToLength(chunkedInstruction.OpCode, 6));
                    bufferBuilder.Append('^');
                    instructionCount++;
                }
            }

            return bufferBuilder.ToString();
        }

        /// <summary>
        ///     Builds the end record for an object program.
        /// </summary>
        public static string BuildEndRecord(IOrderedEnumerable<Instruction> instructions)
        {
            if (instructions == null) throw new ArgumentNullException(nameof(instructions));
            var firstValidInstruction =
                instructions.FirstOrDefault(x => x.Mnemonic.Name != MnemonicList.StartInstruction.Name);
            if (firstValidInstruction == null)
                throw new InvalidOperationException(
                    "A valid instruction was not given during the build of end record.");
            var formattedMemoryLocation = FormatHexToLength(firstValidInstruction.MemoryLocation, 6);
            return $"E^{formattedMemoryLocation}";
        }

        /// <summary>
        ///     Gets instructions whose name matches set declaration instructions.
        /// </summary>
        public static IReadOnlyCollection<Instruction> GetSymbolInstructions(IEnumerable<Instruction> instructions)
            => instructions.Where(x => MnemonicList.SymbolInstructions.Any(d => d.Name == x.Mnemonic.Name))
                .ToImmutableArray();

        /// <summary>
        ///     Formats the hexadecimal string to comply with the specified length.
        /// </summary>
        /// <param name="input">A hexadecimal string.</param>
        /// <param name="length">The specified length to fit the string in.</param>
        /// <returns>
        ///     A padded string if the length of the string is shorter than <paramref name="length" />; otherwise
        ///     <paramref name="input" />.
        /// </returns>
        public static string FormatHexToLength(string input, int length)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (!int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
                throw new InvalidOperationException("The input string is not a valid hexadecimal string.");
            return input.Length > length ? input : input.PadLeft(length, '0');
        }

        /// <summary>
        ///     Parses the raw string into a strongly-typed <see cref="Instruction" /> class.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Instruction ParseInstructions(string input)
        {
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                var rawInstruction = input.Split(Delimiters)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToList();

                Log.Verbose("Parsed {Count} instruction entries from input.", rawInstruction.Count);

                string instructionName;
                Mnemonic mnemonic;
                Instruction instruction;
                switch (rawInstruction.Count)
                {
                    // assume a non-standard instruction (no byte specification)
                    case 1:
                        instructionName = rawInstruction[0];
                        mnemonic = GetMnemonic(instructionName);
                        instruction = new Instruction {Mnemonic = mnemonic};
                        break;
                    // assume standard instruction
                    case 2:
                        instructionName = rawInstruction[0];
                        mnemonic = GetMnemonic(instructionName);
                        instruction = new Instruction {Mnemonic = mnemonic, Parameter = rawInstruction[1]};
                        break;
                    case 3:
                        var instructionLabel = rawInstruction[0];
                        instructionName = rawInstruction[1];
                        var instructionParameter = rawInstruction[2];
                        mnemonic = GetMnemonic(instructionName);
                        instruction = new Instruction
                            {Label = instructionLabel, Mnemonic = mnemonic, Parameter = instructionParameter};
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rawInstruction),
                            $"Compiler error: SIC instruction count mismatch; see line \"{rawInstruction}\"");
                }

                if (instruction.Mnemonic.Name == MnemonicList.StartInstruction.Name)
                    instruction.MemoryLocation = (string) instruction.Parameter;

                Log.Verbose("Instruction \"{Name}\" successfully parsed.", instruction.Mnemonic.Name);
                Log.Verbose("----Instruction label: {Label}", instruction.Label);
                Log.Verbose("----Instruction parameter: {Parameter}", instruction.Parameter);

                return instruction;
            }
        }

        public static Mnemonic GetMnemonic(string instructionName)
            => MnemonicList.StandardInstructions.SingleOrDefault(x => x.Name == instructionName) ??
               MnemonicList.SymbolInstructions.SingleOrDefault(x => x.Name == instructionName) ??
               new Mnemonic {Name = instructionName};
    }
}