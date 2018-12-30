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
using Serilog.Sinks.SystemConsole.Themes;

namespace SICCSharp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] [{Method}] {Message}{NewLine}")
                .CreateLogger();
            CompileHelper.CompileAsync("C:\\Users\\34146\\source\\repos\\SICC.Sharp\\SICCSharp.Test\\source2.asm", null)
                .GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }

    public static class CompileHelper
    {
        private static char[] Delimiter
            => new[] {'\u0009', ' '};

        private static string GetMethodName([CallerMemberName] string name = null)
            => name;

        public static async Task CompileAsync(string input, string output)
        {
            using (var inputStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, 1024,
                FileOptions.Asynchronous))
            using (var inputFile = new StreamReader(inputStream))
            using (LogContext.PushProperty("Method", GetMethodName()))
            {
                Log.Verbose($"Loaded file {input}!");

                string line;
                var lineCounter = 1;
                string baseMemory = null;
                var memoryBufferHex = -1;
                var locationBufferHex = -1;
                var instructionList = new List<Instruction>();
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
                            CalculateLocationOffset(instruction.Mnemonic.Name, instructionParameter);
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
                    Log.Information("{MemoryLocation} - {Label} - {Parameter}", instruction.MemoryLocation,
                        instruction.Label, instruction.Parameter);

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
                            var charArray = CalculateOpCodeBitArray(variableInstruction.MemoryLocation);
                            charArray[0] = '1';
                            var charArrayString = new string(charArray);
                            var targetAddress = Convert.ToInt32(charArrayString, 2).ToString("X");
                            instruction.OpCode = instruction.Mnemonic.OpCode + targetAddress;
                        }
                    }
                    // direct mode
                    else
                    {
                        Instruction targetVariable = null;
                        if (instructionParameter != null)
                        {
                            targetVariable = instructionList.FirstOrDefault(x => x.Label == instructionParameter);
                            if (instruction.Mnemonic.Name == "WORD")
                            {
                                instruction.OpCode = instructionParameter.PadLeft(6,'0');
                                continue;
                            }

                            if (targetVariable == null)
                            {
                                Log.Warning(
                                    "Cannot find equivalent variable in SYMTAB for instruction {Name} on line {LineCount}",
                                    instruction.Mnemonic.Name, instruction.LineCount);
                                continue;
                            }
                        }
                        instruction.OpCode = (instruction.Mnemonic.OpCode + targetVariable?.MemoryLocation).PadRight(6,'0');
                    }
                }

                // print overall
                var summaryBuilder = new StringBuilder(Environment.NewLine)
                    .AppendLine("|LINE|\u0009|LOC|\u0009|LAB|\u0009|INST|\u0009|PARAM|\u0009|OPCODE|");
                foreach (var instruction in instructionList)
                {
                    summaryBuilder.Append($"{instruction.LineCount:D6}:\u0009");
                    summaryBuilder.Append(MnemonicList.InstructionWhitelist.All(x => x != instruction.Mnemonic.Name)
                        ? instruction.MemoryLocation
                        : "----");
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
            }
        }


        public static char[] CalculateOpCodeBitArray(string opCode)
        {
            var binString = string.Join("",
                opCode.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')));
            return binString.ToCharArray();
        }

        public static string CalculateLocationOffset(string instructionName, int? parameter = null)
        {
            switch (instructionName)
            {
                case "START":
                case "END":
                    return "0";
                case "RESW":
                    return parameter != null ? (3 * parameter.Value).ToString("X") : "0";
                case "RESB":
                    return parameter != null ? parameter.Value.ToString("X") : "0";
                default:
                    return "3";
            }
        }

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
                        var instructionLabel = rawInstruction[0];
                        instructionName = rawInstruction[1];
                        var instructionParameter = rawInstruction[2];
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