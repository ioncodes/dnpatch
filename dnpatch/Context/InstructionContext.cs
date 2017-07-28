using dnpatch.Processors;

namespace dnpatch.Context
{
    /// <summary>
    /// Partial Assembly
    /// </summary>
    public partial class Assembly
	{
        /// <summary>
        /// InstructionProcessor context
        /// </summary>
        /// <seealso cref="dnpatch.Processors.InstructionProcessor" />
        public class InstructionContext : InstructionProcessor
		{
			internal InstructionContext(Types.Assembly assembly) : base(assembly)
			{
				Assembly = assembly;
			}
		}
	}
}
