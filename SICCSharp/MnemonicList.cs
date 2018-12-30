using System.Collections.Generic;

namespace SICCSharp
{
    public static class MnemonicList
    {
        public static IReadOnlyCollection<Mnemonic> Instructions = new []
        {
            new Mnemonic{Name = "ADD",  Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "18"},
            new Mnemonic{Name = "ADDF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "58"}, 
            new Mnemonic{Name = "ADDR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "90"}, 
            new Mnemonic{Name = "AND",  Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "40"}, 
            new Mnemonic{Name = "CLEAR",Format = new []{MnemonicFormat.TwoBytes}, OpCode = "B4"}, 
            new Mnemonic{Name = "COMP", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "28"}, 
            new Mnemonic{Name = "COMPF",Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "88"}, 
            new Mnemonic{Name = "COMPR",Format = new []{MnemonicFormat.TwoBytes}, OpCode = "A0"}, 
            new Mnemonic{Name = "DIV", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "24"}, 
            new Mnemonic{Name = "DIVF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "64"}, 
            new Mnemonic{Name = "DIVR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "9C"}, 
            new Mnemonic{Name = "FIX", Format = new []{MnemonicFormat.OneByte}, OpCode = "C4"}, 
            new Mnemonic{Name = "FLOAT", Format = new []{MnemonicFormat.OneByte}, OpCode = "C0"}, 
            new Mnemonic{Name = "HIO", Format = new []{MnemonicFormat.OneByte}, OpCode = "F4"}, 
            new Mnemonic{Name = "J", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "3C"}, 
            new Mnemonic{Name = "JEQ", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "30"}, 
            new Mnemonic{Name = "JGT", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "34"}, 
            new Mnemonic{Name = "JLT", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "38"}, 
            new Mnemonic{Name = "JSUB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "48"}, 
            new Mnemonic{Name = "LDA", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "00"}, 
            new Mnemonic{Name = "LDB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "68"}, 
            new Mnemonic{Name = "LDCH", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "50"}, 
            new Mnemonic{Name = "LDF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "70"}, 
            new Mnemonic{Name = "LDL", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "08"}, 
            new Mnemonic{Name = "LDS", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "6C"}, 
            new Mnemonic{Name = "LDT", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "74"}, 
            new Mnemonic{Name = "LDX", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "04"}, 
            new Mnemonic{Name = "LPS", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "D0"}, 
            new Mnemonic{Name = "MUL", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "20"}, 
            new Mnemonic{Name = "MULF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "60"}, 
            new Mnemonic{Name = "MULR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "98"}, 
            new Mnemonic{Name = "NORM", Format = new []{MnemonicFormat.OneByte}, OpCode = "C8"}, 
            new Mnemonic{Name = "OR", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "44"}, 
            new Mnemonic{Name = "RD", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "D8"}, 
            new Mnemonic{Name = "RMO", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "AC"}, 
            new Mnemonic{Name = "RSUB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "4C"}, 
            new Mnemonic{Name = "SHIFTL", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "A4"}, 
            new Mnemonic{Name = "SHIFTR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "A8"}, 
            new Mnemonic{Name = "SIO", Format = new []{MnemonicFormat.OneByte}, OpCode = "F0"}, 
            new Mnemonic{Name = "SSK", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "EC"}, 
            new Mnemonic{Name = "STA", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "0C"}, 
            new Mnemonic{Name = "STB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "78"}, 
            new Mnemonic{Name = "STCH", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "54"}, 
            new Mnemonic{Name = "STF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "80"}, 
            new Mnemonic{Name = "STI", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "D4"}, 
            new Mnemonic{Name = "STL", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "14"}, 
            new Mnemonic{Name = "STS", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "7C"}, 
            new Mnemonic{Name = "STSW", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "E8"}, 
            new Mnemonic{Name = "STT", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "84"}, 
            new Mnemonic{Name = "STX", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "10"}, 
            new Mnemonic{Name = "SUB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "1C"}, 
            new Mnemonic{Name = "SUBF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "5C"}, 
            new Mnemonic{Name = "SUBR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "94"}, 
            new Mnemonic{Name = "SVC", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "B0"}, 
            new Mnemonic{Name = "TD", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "E0"}, 
            new Mnemonic{Name = "TIO", Format = new []{MnemonicFormat.OneByte}, OpCode = "F8"}, 
            new Mnemonic{Name = "TIX", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "2C"}, 
            new Mnemonic{Name = "TIXR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = "B8"}, 
            new Mnemonic{Name = "WD", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "DC"}, 
        };

        public static string[] InstructionWhitelist
            => new[] {"START", "END"};

        public static IReadOnlyCollection<Mnemonic> DeclarationInstructions
            => new[]
            {
                new Mnemonic{Name = "RESW"}, 
                new Mnemonic{Name = "RESB"}, 
                new Mnemonic{Name = "WORD"}, 
            };
    }
}