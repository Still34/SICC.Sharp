namespace SICCSharp.Entities
{
    public class Mnemonic
    {
        public string Name { get; set; }
        public MnemonicFormat[] Format { get; set; } = {MnemonicFormat.NotApplicable};
        public string OpCode { get; set; } = "00";

        public override string ToString()
            => Name;
    }
}