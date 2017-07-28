using dnlib.DotNet.Emit;

namespace dnpatch.Types
{
    // Combines Instructions and Indices. If Indices equals null, ignore them.
    // TODO: Im not sure if we should stick to this idea. Or even force this type?
    /// <summary>
    /// The instruction and index set
    /// </summary>
    public struct InstructionSet
    {
        /// <summary>
        /// Gets or sets the instructions.
        /// </summary>
        /// <value>
        /// The instructions.
        /// </value>
        public Instruction[] Instructions { get; set; }
        /// <summary>
        /// Gets or sets the indices.
        /// </summary>
        /// <value>
        /// The indices.
        /// </value>
        public int[] Indices { get; set; }
    }
}