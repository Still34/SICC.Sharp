using System.Collections.Generic;

namespace SICCSharp
{
    public static class MnemonicList
    {
        public static IReadOnlyCollection<Mnemonic> Instructions = new []
        {
            new Mnemonic{Name = "ADD",  Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x18},
            new Mnemonic{Name = "ADDF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x58}, 
            new Mnemonic{Name = "ADDR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0x90}, 
            new Mnemonic{Name = "AND",  Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x40}, 
            new Mnemonic{Name = "CLEAR",Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0xB4}, 
            new Mnemonic{Name = "COMP", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x28}, 
            new Mnemonic{Name = "COMPF",Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x88}, 
            new Mnemonic{Name = "COMPR",Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0xA0}, 
            new Mnemonic{Name = "DIV", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x24}, 
            new Mnemonic{Name = "DIVF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x64}, 
            new Mnemonic{Name = "DIVR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0x9C}, 
            new Mnemonic{Name = "FIX", Format = new []{MnemonicFormat.OneByte}, OpCode = 0xC4}, 
            new Mnemonic{Name = "FLOAT", Format = new []{MnemonicFormat.OneByte}, OpCode = 0xC0}, 
            new Mnemonic{Name = "HIO", Format = new []{MnemonicFormat.OneByte}, OpCode = 0xF4}, 
            new Mnemonic{Name = "J", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x3C}, 
            new Mnemonic{Name = "JEQ", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x30}, 
            new Mnemonic{Name = "JGT", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x34}, 
            new Mnemonic{Name = "JLT", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x38}, 
            new Mnemonic{Name = "JSUB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x48}, 
            new Mnemonic{Name = "LDA", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x00}, 
            new Mnemonic{Name = "LDB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x68}, 
            new Mnemonic{Name = "LDCH", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x50}, 
            new Mnemonic{Name = "LDF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x70}, 
            new Mnemonic{Name = "LDL", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x08}, 
            new Mnemonic{Name = "LDS", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x6C}, 
            new Mnemonic{Name = "LDT", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x74}, 
            new Mnemonic{Name = "LDX", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x04}, 
            new Mnemonic{Name = "LPS", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0xD0}, 
            new Mnemonic{Name = "MUL", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x20}, 
            new Mnemonic{Name = "MULF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x60}, 
            new Mnemonic{Name = "MULR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0x98}, 
            new Mnemonic{Name = "NORM", Format = new []{MnemonicFormat.OneByte}, OpCode = 0xC8}, 
            new Mnemonic{Name = "OR", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x44}, 
            new Mnemonic{Name = "RD", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0xD8}, 
            new Mnemonic{Name = "RMO", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0xAC}, 
            new Mnemonic{Name = "RSUB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x4C}, 
            new Mnemonic{Name = "SHIFTL", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0xA4}, 
            new Mnemonic{Name = "SHIFTR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0xA8}, 
            new Mnemonic{Name = "SIO", Format = new []{MnemonicFormat.OneByte}, OpCode = 0xF0}, 
            new Mnemonic{Name = "SSK", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0xEC}, 
            new Mnemonic{Name = "STA", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x0C}, 
            new Mnemonic{Name = "STB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x78}, 
            new Mnemonic{Name = "STCH", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x54}, 
            new Mnemonic{Name = "STF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x80}, 
            new Mnemonic{Name = "STI", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0xD4}, 
            new Mnemonic{Name = "STL", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x14}, 
            new Mnemonic{Name = "STS", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x7C}, 
            new Mnemonic{Name = "STSW", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0xE8}, 
            new Mnemonic{Name = "STT", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x84}, 
            new Mnemonic{Name = "STX", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x10}, 
            new Mnemonic{Name = "SUB", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x1C}, 
            new Mnemonic{Name = "SUBF", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x5C}, 
            new Mnemonic{Name = "SUBR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0x94}, 
            new Mnemonic{Name = "SVC", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0xB0}, 
            new Mnemonic{Name = "TD", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0xE0}, 
            new Mnemonic{Name = "TIO", Format = new []{MnemonicFormat.OneByte}, OpCode = 0xF8}, 
            new Mnemonic{Name = "TIX", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0x2C}, 
            new Mnemonic{Name = "TIXR", Format = new []{MnemonicFormat.TwoBytes}, OpCode = 0xB8}, 
            new Mnemonic{Name = "WD", Format = new []{MnemonicFormat.ThreeBytes, MnemonicFormat.FourBytes}, OpCode = 0xDC}, 
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