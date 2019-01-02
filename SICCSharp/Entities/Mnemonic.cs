namespace SICCSharp.Entities
{
    /// <summary>
    ///     Represents a generic mnemonic instruction.
    /// </summary>
    public class Mnemonic
    {
        /// <summary>
        ///     Gets or sets the name of this mnemonic.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the formats this mnemonic uses.
        /// </summary>
        /// <remarks>This property is currently unused, as it is only valid under SIC/XE compilation.</remarks>
        public MnemonicFormat[] Formats { get; set; } = {MnemonicFormat.NotApplicable};

        /// <summary>
        ///     Gets or sets the operation code for this mnemonic.
        /// </summary>
        public string OpCode { get; set; } = "00";

        /// <summary>
        ///     Gets the name of this mnemonic.
        /// </summary>
        public override string ToString()
            => Name;
    }
}