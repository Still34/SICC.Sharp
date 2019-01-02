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
        private const char TabChar = '\u0009';

        private static char[] Delimiter
            => new[] {TabChar, ' '};

        private static string GetMethodName([CallerMemberName] string name = null)
            => name;

        public static async Task CompileAsync(string input, string output)
        {
            using (var inputStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, 128,
                FileOptions.Asynchronous))
            using (var inputFile = new StreamReader(inputStream))
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                Log.Verbose($"Loaded file {input}...");

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
                        var instructionParameter = int.TryParse((string) instruction.Parameter, out var instructionInt)
                            ? (int?) instructionInt
                            : null;

                        var locationOffset =
                            MathHelper.CalculateLocationOffset(instruction.Mnemonic.Name, instructionParameter);
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
                var symTab = GetSymbolInstructions(instructionList);
                foreach (var instruction in symTab)
                    Log.Information("{MemoryLocation} - {Label} - {Parameter}", instruction.MemoryLocation,
                        instruction.Label, instruction.Parameter);

                // Second pass
                // compile obj code
                foreach (var instruction in instructionList)
                {
                    // ignore if it's one of the whitelisted instructions
                    if (MnemonicList.InstructionWhitelist.Any(x => x == instruction.Mnemonic.Name)) continue;
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
                var summaryBuilder = new StringBuilder(Environment.NewLine);
                foreach (var instruction in instructionList)
                {
                    summaryBuilder.Append(MnemonicList.InstructionWhitelist.All(x => x != instruction.Mnemonic.Name)
                        ? instruction.MemoryLocation
                        : TabChar.ToString());

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

                Log.Information(summaryBuilder.ToString());

                // write ASM output inc. object code and location table
                var asmOutput = Path.Combine(output, "output.txt");
                await File.WriteAllTextAsync(asmOutput, summaryBuilder.ToString());

                // compile object program
                Log.Information(CompileObjectProgram(instructionList));

                Log.Information("Saved ASM output to {asmOutput}", asmOutput);
            }
        }

        public static string CompileObjectProgram(IEnumerable<Instruction> instructions)
        {
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                // conditional checks
                if (instructions == null) throw new ArgumentNullException(nameof(instructions));
                var orderedInstructions = instructions.OrderBy(x => x.LineCount).ToList();
                if (!orderedInstructions.Any()) throw new InvalidOperationException("Instruction size zero.");

                // init builder
                var objectProgramBuilder = new StringBuilder();

                // get header name; otherwise default to PROG
                var firstInstruction = orderedInstructions.FirstOrDefault();
                var lastInstruction = orderedInstructions.LastOrDefault();
                var headerName = firstInstruction?.Mnemonic.Name == "START" ? firstInstruction.Label : "PROG";
                objectProgramBuilder.Append($"H^{headerName}");

                // get first line and last line to get total memory content
                objectProgramBuilder.Append($"^{FormatHexToLength(firstInstruction?.MemoryLocation, 6)}");
                objectProgramBuilder.Append(
                    $"^{FormatHexToLength(MathHelper.SubHex(lastInstruction?.MemoryLocation, firstInstruction?.MemoryLocation), 6)}");

                var bufferBuilder = new StringBuilder();
                var commandInstructions = new List<Instruction>();
                var symbolInstructions = new List<Instruction>();
                foreach (var instruction in orderedInstructions)
                {
                    if (MnemonicList.Instructions.Any(x => x.OpCode == instruction.Mnemonic.OpCode))
                        commandInstructions.Add(instruction);
                    else if (MnemonicList.DeclarationInstructions.Any(x => x.Name == instruction.Mnemonic.Name))
                        symbolInstructions.Add(instruction);
                }



                return objectProgramBuilder.ToString();
            }
        }

        public static IReadOnlyCollection<Instruction> GetSymbolInstructions(IEnumerable<Instruction> instructions)
            => instructions.Where(x => MnemonicList.DeclarationInstructions.Any(d => d.Name == x.Mnemonic.Name))
                .ToImmutableArray();

        public static string FormatHexToLength(string input, int length)
            => input.PadLeft(length, '0');

        public static Instruction ParseInstructions(string input)
        {
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                var rawInstruction = input.Split(Delimiter)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToList();

                Log.Verbose("Parsed {Count} instructions from input.", rawInstruction.Count);

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