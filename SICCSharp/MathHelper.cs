using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SICCSharp
{
    public static class MathHelper
    {
        public static string AddHex(string inputOne, string inputTwo) => (Int32.Parse(inputOne, NumberStyles.HexNumber) + Int32.Parse(inputTwo, NumberStyles.HexNumber)).ToString("X");
        public static string SubHex(string inputOne, string inputTwo) => (Int32.Parse(inputOne, NumberStyles.HexNumber) - Int32.Parse(inputTwo, NumberStyles.HexNumber)).ToString("X");

        public static char[] CalculateOpCodeBitArray(string opCode) => String.Join("",
            opCode.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0'))).ToArray();

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
    }
}
