using dnlib.DotNet.Emit;

namespace dnpatch.Types
{
    // Combines Instructions and Indices. If Indices equals null, ignore them.
    // TODO: Im not sure if we should stick to this idea. Or even force this type?
    public struct InstructionSet
    {
        public Instruction[] Instructions { get; set; }
        public int[] Indices { get; set; }
    }
}