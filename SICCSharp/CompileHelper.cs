using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Context;

namespace SICCSharp
{
    public static class CompileHelper
    {
        private static char[] Delimiter
            => new[] {'\u0009', ' '};

        private static string GetMethodName([CallerMemberName] string name = null)
            => name;

        public static async Task CompileAsync(string input, string output)
        {
            using (var inputStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, 128, FileOptions.Asynchronous))
            using (var inputFile = new StreamReader(inputStream))
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                Log.Verbose($"Loaded file {input}...");

                string line;
                int lineCounter = 1;
                string baseMemory = null;
                int memoryBufferHex = -1;
                int locationBufferHex = -1;
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
                    if (instruction.Mnemonic.Name == "START")
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
                        var instructionParameter = int.TryParse((string) instruction.Parameter, out int instructionInt)
                            ? (int?) instructionInt
                            : null;

                        string locationOffset = MathHelper.CalculateLocationOffset(instruction.Mnemonic.Name, instructionParameter);
                        memoryBufferHex = int.Parse(baseMemory, NumberStyles.HexNumber);
                        locationBufferHex = int.Parse(locationOffset, NumberStyles.HexNumber);
                        instruction.MemoryLocation = baseMemory;
                    }

                    instruction.LineCount = lineCounter;
                    instructionList.Add(instruction);
                    lineCounter++;
                }

                // print SYMTAB
                Log.Information("SYMTAB (Symbol Table)");
                var symTab = instructionList.Where(x
                    => MnemonicList.DeclarationInstructions.Any(d => d.Name == x.Mnemonic.Name)).ToList();
                foreach (var instruction in symTab)
                {
                    Log.Information("{MemoryLocation} - {Label} - {Parameter}", instruction.MemoryLocation,
                        instruction.Label, instruction.Parameter);
                }

                // Second pass
                // compile obj code
                foreach (var instruction in instructionList)
                {
                    // ignore if it's one of the whitelisted instructions
                    if (MnemonicList.InstructionWhitelist.Any(x => x == instruction.Mnemonic.Name)) continue;
                    string instructionParameter = (string) instruction.Parameter;
                    var splitParameter = instructionParameter?.Split(',');
                    // indexed mode
                    if (splitParameter?.Length > 1)
                    {
                        string targetVariable = splitParameter[0];
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
                            string charArrayString = new string(charArray);
                            string targetAddress = Convert.ToInt32(charArrayString, 2).ToString("X");
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
                                instruction.OpCode = FormatHexToLength(instructionParameter,6);
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
                var summaryBuilder = new StringBuilder(Environment.NewLine);
                foreach (var instruction in instructionList)
                {
                    summaryBuilder.Append(MnemonicList.InstructionWhitelist.All(x => x != instruction.Mnemonic.Name)
                        ? instruction.MemoryLocation
                        : "\u0009");
                    summaryBuilder.Append(!string.IsNullOrEmpty(instruction.Label)
                        ? $"\u0009{instruction.Label}"
                        : "\u0009");
                    summaryBuilder.Append(!string.IsNullOrEmpty(instruction.Mnemonic.Name)
                        ? $"\u0009{instruction.Mnemonic}"
                        : "\u0009");
                    summaryBuilder.Append(!string.IsNullOrEmpty((string) instruction.Parameter)
                        ? $"\u0009{instruction.Parameter}"
                        : "\u0009");
                    summaryBuilder.Append(!string.IsNullOrEmpty(instruction.OpCode)
                        ? $"\u0009{instruction.OpCode}"
                        : "\u0009");
                    summaryBuilder.AppendLine();
                }

                Log.Information(summaryBuilder.ToString());

                string asmOutput = Path.Combine(output, "output.asm");
                await File.WriteAllTextAsync(asmOutput, summaryBuilder.ToString());

                Log.Information(CompileObjectProgram(instructionList));

                Log.Information("Saved ASM output to {asmOutput}", asmOutput);
            }
        }

        public static string CompileObjectProgram(IEnumerable<Instruction> instructions)
        {
            // conditional checks
            if (instructions == null) throw new ArgumentNullException(nameof(instructions));
            var orderedInstructions = instructions.OrderBy(x => x.LineCount).ToList();
            if (!orderedInstructions.Any()) throw new InvalidOperationException("Instruction size zero.");

            // init builder
            var objectProgramBuilder = new StringBuilder();

            // get header name; otherwide default to PROG
            var firstInstruction = orderedInstructions.FirstOrDefault();
            var lastInstruction = orderedInstructions.LastOrDefault();
            string headerName = firstInstruction?.Mnemonic.Name == "START" ? firstInstruction.Label : "PROG";
            objectProgramBuilder.Append($"H^{headerName}");

            // get first line and last line to get total memory content
            objectProgramBuilder.Append($"^{(FormatHexToLength(firstInstruction?.MemoryLocation, 6))}");
            objectProgramBuilder.Append(
                $"^{FormatHexToLength(MathHelper.SubHex(lastInstruction?.MemoryLocation, firstInstruction?.MemoryLocation), 6)}");



            return objectProgramBuilder.ToString();
        }

        public static string FormatHexToLength(string input, int length) => input.PadLeft(length, '0');

        public static Instruction ParseInstructions(string input)
        {
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                var rawInstruction = input.Split(Delimiter)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToList();

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
                        string instructionLabel = rawInstruction[0];
                        instructionName = rawInstruction[1];
                        string instructionParameter = rawInstruction[2];
                        mnemonic = GetMnemonic(instructionName);
                        instruction = new Instruction
                            {Label = instructionLabel, Mnemonic = mnemonic, Parameter = instructionParameter};
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rawInstruction),
                            "Invalid argument count for instruction.");
                }

                if (instruction.Mnemonic.Name == "START") instruction.MemoryLocation = (string) instruction.Parameter;

                Log.Verbose("Instruction \"{Name}\" successfully parsed.", instruction.Mnemonic.Name);
                Log.Verbose("----Instruction label: {Label}", instruction.Label);
                Log.Verbose("----Instruction parameter: {Parameter}", instruction.Parameter);

                return instruction;
            }
        }

        public static Mnemonic GetMnemonic(string instructionName)
            => MnemonicList.Instructions.SingleOrDefault(x => x.Name == instructionName) ??
               MnemonicList.DeclarationInstructions.SingleOrDefault(x => x.Name == instructionName) ??
               new Mnemonic {Name = instructionName};
    }
}