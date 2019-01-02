using System;
using System.Globalization;
using System.Linq;
using SICCSharp.Entities;

namespace SICCSharp.Extensions
{
    public static class MathHelper
    {
        public static string AddHex(string inputOne, string inputTwo) => (Int32.Parse(inputOne, NumberStyles.HexNumber) + Int32.Parse(inputTwo, NumberStyles.HexNumber)).ToString("X");
        public static string SubHex(string inputOne, string inputTwo) => (Int32.Parse(inputOne, NumberStyles.HexNumber) - Int32.Parse(inputTwo, NumberStyles.HexNumber)).ToString("X");

        public static char[] CalculateOpCodeBitArray(string opCode) => String.Join("",
            opCode.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))).ToArray();

        public static string CalculateLocationOffset(Instruction instruction)
        {
            if (instruction.Parameter == null || !int.TryParse((string) instruction.Parameter, out var instructionInt)) return "3";
            switch (instruction.Mnemonic.Name)
            {
                case var n when n == MnemonicList.StartInstruction.Name:
                case var e when e == MnemonicList.EndInstruction.Name:
                    return "0";
                case "RESW":
                    return (3 * instructionInt).ToString("X");
                case "RESB":
                    return instructionInt.ToString("X");
                default:
                    return "3";
            }
        }
    }
}
