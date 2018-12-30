using System;

namespace SICCSharp
{
    public class Instruction
    {
        public int LineCount { get; set; }
        public string MemoryLocation { get; set; }
        public string Label { get; set; }
        public Mnemonic Mnemonic { get; set; }
        public object Parameter { get; set; }
        public string OpCode { get; set; }
    }
}