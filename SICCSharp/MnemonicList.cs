using System.Collections.Generic;
using SICCSharp.Entities;

namespace SICCSharp
{
    public static class MnemonicList
    {
        public static IReadOnlyCollection<Mnemonic> StandardInstructions = new []
        {
            new Mnemonic{Name = "ADD",  Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "18"},
            new Mnemonic{Name = "ADDF", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "58"}, 
            new Mnemonic{Name = "ADDR", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "90"}, 
            new Mnemonic{Name = "AND",  Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "40"}, 
            new Mnemonic{Name = "CLEAR",Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "B4"}, 
            new Mnemonic{Name = "COMP", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "28"}, 
            new Mnemonic{Name = "COMPF",Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "88"}, 
            new Mnemonic{Name = "COMPR",Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "A0"}, 
            new Mnemonic{Name = "DIV", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "24"}, 
            new Mnemonic{Name = "DIVF", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "64"}, 
            new Mnemonic{Name = "DIVR", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "9C"}, 
            new Mnemonic{Name = "FIX", Formats = new []{MnemonicFormat.OneByte}, OpCode = "C4"}, 
            new Mnemonic{Name = "FLOAT", Formats = new []{MnemonicFormat.OneByte}, OpCode = "C0"}, 
            new Mnemonic{Name = "HIO", Formats = new []{MnemonicFormat.OneByte}, OpCode = "F4"}, 
            new Mnemonic{Name = "J", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "3C"}, 
            new Mnemonic{Name = "JEQ", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "30"}, 
            new Mnemonic{Name = "JGT", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "34"}, 
            new Mnemonic{Name = "JLT", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "38"}, 
            new Mnemonic{Name = "JSUB", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "48"}, 
            new Mnemonic{Name = "LDA", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "00"}, 
            new Mnemonic{Name = "LDB", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "68"}, 
            new Mnemonic{Name = "LDCH", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "50"}, 
            new Mnemonic{Name = "LDF", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "70"}, 
            new Mnemonic{Name = "LDL", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "08"}, 
            new Mnemonic{Name = "LDS", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "6C"}, 
            new Mnemonic{Name = "LDT", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "74"}, 
            new Mnemonic{Name = "LDX", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "04"}, 
            new Mnemonic{Name = "LPS", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "D0"}, 
            new Mnemonic{Name = "MUL", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "20"}, 
            new Mnemonic{Name = "MULF", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "60"}, 
            new Mnemonic{Name = "MULR", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "98"}, 
            new Mnemonic{Name = "NORM", Formats = new []{MnemonicFormat.OneByte}, OpCode = "C8"}, 
            new Mnemonic{Name = "OR", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "44"}, 
            new Mnemonic{Name = "RD", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "D8"}, 
            new Mnemonic{Name = "RMO", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "AC"}, 
            new Mnemonic{Name = "RSUB", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "4C"}, 
            new Mnemonic{Name = "SHIFTL", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "A4"}, 
            new Mnemonic{Name = "SHIFTR", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "A8"}, 
            new Mnemonic{Name = "SIO", Formats = new []{MnemonicFormat.OneByte}, OpCode = "F0"}, 
            new Mnemonic{Name = "SSK", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "EC"}, 
            new Mnemonic{Name = "STA", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "0C"}, 
            new Mnemonic{Name = "STB", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "78"}, 
            new Mnemonic{Name = "STCH", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "54"}, 
            new Mnemonic{Name = "STF", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "80"}, 
            new Mnemonic{Name = "STI", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "D4"}, 
            new Mnemonic{Name = "STL", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "14"}, 
            new Mnemonic{Name = "STS", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "7C"}, 
            new Mnemonic{Name = "STSW", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "E8"}, 
            new Mnemonic{Name = "STT", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "84"}, 
            new Mnemonic{Name = "STX", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "10"}, 
            new Mnemonic{Name = "SUB", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "1C"}, 
            new Mnemonic{Name = "SUBF", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "5C"}, 
            new Mnemonic{Name = "SUBR", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "94"}, 
            new Mnemonic{Name = "SVC", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "B0"}, 
            new Mnemonic{Name = "TD", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "E0"}, 
            new Mnemonic{Name = "TIO", Formats = new []{MnemonicFormat.OneByte}, OpCode = "F8"}, 
            new Mnemonic{Name = "TIX", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "2C"}, 
            new Mnemonic{Name = "TIXR", Formats = new []{MnemonicFormat.TwoBytes}, OpCode = "B8"}, 
            new Mnemonic{Name = "WD", Formats = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = "DC"}, 
        };

        public static IReadOnlyCollection<Mnemonic> BeginEndInstructions
            => new[] {StartInstruction, EndInstruction};

        public static Mnemonic StartInstruction
            => new Mnemonic{Name = "START"};

        public static Mnemonic EndInstruction
            => new Mnemonic{Name = "END"};

        public static IReadOnlyCollection<Mnemonic> SymbolInstructions
            => new[]
            {
                new Mnemonic{Name = "RESW"}, 
                new Mnemonic{Name = "RESB"}, 
                new Mnemonic{Name = "WORD"}, 
            };
    }
}