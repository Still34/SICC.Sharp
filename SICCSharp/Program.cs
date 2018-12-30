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
                .WriteTo.Console(theme: AnsiConsoleTheme.Code,outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] [{Method}] {Message}{NewLine}")
                .CreateLogger();
            CompileHelper.CompileAsync("C:\\Users\\34146\\source\\repos\\SICC.Sharp\\SICCSharp.Test\\source.asm", null)
                .GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }

    public static class CompileHelper
    {
        private static string GetMethodName([CallerMemberName]string name = null) => name;
        private static char[] Delimiter
            => new[] {'\u0009', ' '};

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
                List<Instruction> instructionList = new List<Instruction>();
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
                            ? (int?)instructionInt
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

                // compile obj code
                foreach (var instruction in instructionList)
                {
                    // ignore if it's one of the whitelisted instructions
                    if (MnemonicList.InstructionWhitelist.Any(x=>x == instruction.Mnemonic.Name)) continue;
                }

                // print SYMTAB
                Log.Information("SYMTAB (Symbol Table)");
                foreach (var instruction in instructionList.Where(x=>MnemonicList.DeclarationInstructions.Any(d=>d.Name == x.Mnemonic.Name)))
                {
                    Log.Information("{MemoryLocation} - {Label} - {Parameter}", instruction.MemoryLocation,
                        instruction.Label, instruction.Parameter);
                }

                // print overall
                var summaryBuilder = new StringBuilder(Environment.NewLine)
                    .AppendLine("|LINE|\u0009|LOC|\u0009|LAB|\u0009|INST|\u0009|PARAM|");
                foreach (var instruction in instructionList)
                {
                    summaryBuilder.Append($"{instruction.LineCount:D6}:\u0009");
                    summaryBuilder.Append(MnemonicList.InstructionWhitelist.All(x => x != instruction.Mnemonic.Name)
                        ? instruction.MemoryLocation
                        : "----");
                    if (!string.IsNullOrEmpty(instruction.Label)) summaryBuilder.Append($"\u0009{instruction.Label}");
                    if (!string.IsNullOrEmpty(instruction.Mnemonic.Name)) summaryBuilder.Append($"\u0009{instruction.Mnemonic}");
                    if (!string.IsNullOrEmpty((string)instruction.Parameter)) summaryBuilder.Append($"\u0009{instruction.Parameter}");
                    summaryBuilder.AppendLine();
                }
                Log.Information(summaryBuilder.ToString());
            }
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
                        instruction = new Instruction {Mnemonic = new Mnemonic {Name = rawInstruction[0]}};
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