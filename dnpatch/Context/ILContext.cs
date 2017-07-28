using dnpatch.Processors;

namespace dnpatch.Context
{
    /// <summary>
    /// Partial Assembly
    /// </summary>
    public partial class Assembly
    {
        /// <summary>
        /// ILProcessor context
        /// </summary>
        /// <seealso cref="dnpatch.Processors.ILProcessor" />
        public class ILContext : ILProcessor
        {
            internal ILContext(Types.Assembly assembly) : base(assembly)
            {
                Assembly = assembly;
            }
        }
    }
}
