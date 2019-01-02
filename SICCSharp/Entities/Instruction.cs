namespace SICCSharp.Entities
{
    /// <summary>
    ///     Represents a generic SIC(XE) instruction.
    /// </summary>
    public class Instruction
    {
        /// <summary>
        ///     Gets or sets the line count of which this instruction is located at in the original ASM input. This number starts
        ///     at 1.
        /// </summary>
        public int LineCount { get; internal set; }

        /// <summary>
        ///     Gets or sets the memory location this instruction is located at.
        /// </summary>
        public string MemoryLocation { get; internal set; }

        /// <summary>
        ///     Gets or sets the label this instruction has.
        /// </summary>
        /// <returns>
        ///     A string representing the label of this instruction; <see langword="null" /> if none is set.
        /// </returns>
        public string Label { get; internal set; }

        public Mnemonic Mnemonic { get; internal set; }
        public object Parameter { get; internal set; }
        public string OpCode { get; internal set; }
    }
}