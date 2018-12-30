using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace SICCSharp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Console().CreateLogger();
            CompileHelper.CompileAsync("C:\\Users\\34146\\source\\repos\\SICC.Sharp\\SICCSharp.Test\\source.asm", null)
                .GetAwaiter().GetResult();
            Console.ReadKey();
        }
    }

    public static class CompileHelper
    {
        private static char[] Delimiter
            => new[] {'\u0009', ' '};

        public static async Task CompileAsync(string input, string output)
        {
            using (var inputStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read, 1024,
                FileOptions.Asynchronous))
            using (var inputFile = new StreamReader(inputStream))
            {
                string line;
                var lineCounter = 1;
                string baseMemory = null;
                var locationBuilder = new StringBuilder();
                var memoryBufferHex = -1;
                var locationBufferHex = -1;
                Dictionary<string, (string Location, int VariableValue)> VariableTable = new Dictionary<string, (string Location, int VariableValue)>();
                while (!string.IsNullOrEmpty(line = await inputFile.ReadLineAsync()))
                {
                    // check if base memory was found and set
                    if (baseMemory != null)
                        if (memoryBufferHex != -1)
                            baseMemory = (memoryBufferHex + locationBufferHex).ToString("X");

                    // split current line into label, instruction, parameter (where possible)
                    var splitEntries = line.Split(Delimiter)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Trim())
                        .ToList();
                    string instructionName = null;
                    var instructionParameter = 0;
                    Log.Verbose($"Line {lineCounter} has {splitEntries.Count} entries.");
                    // check if START is present (currently does not support multiple START instruction)
                    if (splitEntries.Any(x=>x == "START"))
                    {
                        baseMemory = splitEntries[splitEntries.Count - 1];
                        Log.Verbose($"Found base memory assignment on line {lineCounter}: {baseMemory}");
                    }
                    // check if baseMemory is set; otherwise assume 0
                    if (baseMemory == null)
                    {
                        baseMemory = "0";
                        Log.Information("Base memory assignment was not found, assuming 0...");
                    }
                    // if baseMemory is set, get current line's memory and location increment as buffer for next line to add
                    else
                    {
                        // log instruction name for instruction + parameter
                        if (splitEntries.Count > 1)
                        {
                            instructionName = splitEntries[splitEntries.Count - 2];
                            int.TryParse(splitEntries[splitEntries.Count - 1], out instructionParameter);
                            if (splitEntries.Count == 3 && MnemonicList.DeclarationInstructions.Any(x=>x.Name == instructionName))
                            {
                                VariableTable.Add(splitEntries[0], (baseMemory, instructionParameter));
                            }
                        }
                        var locationOffset = CalculateLocationOffset(instructionName, instructionParameter);
                        memoryBufferHex = int.Parse(baseMemory, NumberStyles.HexNumber);
                        locationBufferHex = int.Parse(locationOffset, NumberStyles.HexNumber);
                    }

                    Log.Verbose($"Line {lineCounter} has an location address of {baseMemory}.");
                    locationBuilder.Append($"{lineCounter:D4}: ");
                    locationBuilder.Append(MnemonicList.InstructionWhitelist.All(x => x != instructionName) ? baseMemory : "\u0009");
                    locationBuilder.Append($" {line}");
                    locationBuilder.AppendLine();
                    lineCounter++;
                }

                Log.Information(Environment.NewLine + locationBuilder);
                foreach (var keyValuePair in VariableTable)
                {
                    Log.Verbose(
                        $"{keyValuePair.Value.Location} - Variable {keyValuePair.Key} - Value {keyValuePair.Value.VariableValue}");
                }
            }
        }

        public static string CalculateLocationOffset(string instructionName, int? parameter = null)
        {
            switch (instructionName)
            {
                case "START":
                case "END":
                case "RSUB":
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
                    instruction = new Instruction {Mnemonic = new Mnemonic(){Name = rawInstruction[0]}};
                    break;
                // assume standard instruction
                case 2:
                    instructionName = rawInstruction[0];
                    mnemonic = GetMnemonic(instructionName);
                    instruction = new Instruction {Mnemonic =  mnemonic, Parameter = rawInstruction[1]};
                    break;
                case 3:
                    var instructionLabel = rawInstruction[0];
                    instructionName = rawInstruction[1];
                    var instructionParameter = rawInstruction[2];
                    mnemonic = GetMnemonic(instructionName);
                    instruction = new Instruction(){Label = instructionLabel, Mnemonic = mnemonic, Parameter = instructionParameter};
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rawInstruction), "Invalid argument count for instruction.");
            }

            if (instruction.Mnemonic.Name == "START") instruction.MemoryLocation = instruction.Parameter;
            return instruction;
        }

        public static Mnemonic GetMnemonic(string instructionName)
            => MnemonicList.Instructions.SingleOrDefault(x => x.Name == instructionName) ??
               MnemonicList.DeclarationInstructions.SingleOrDefault(x => x.Name == instructionName) ?? 
               new Mnemonic {Name = instructionName};
    }

    public class Instruction
    {
        public string MemoryLocation { get;set; }
        public string Label { get; set; }
        public Mnemonic Mnemonic { get; set; }
        public string Parameter { get;set; }
    }
}