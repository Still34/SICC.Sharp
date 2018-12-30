namespace SICCSharp
{
    public class Mnemonic
    {
        public string Name { get; set; }
        public MnemonicFormat[] Format { get; set; } = {MnemonicFormat.NotApplicable};
        public byte OpCode { get; set; } = 0x00;
    }
}