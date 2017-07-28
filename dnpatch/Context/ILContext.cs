using dnpatch.Processors;

namespace dnpatch.Context
{
    public partial class Assembly
    {
        public class ILContext : ILProcessor
        {
            internal ILContext(Types.Assembly assembly) : base(assembly)
            {
                _assembly = assembly;
            }
        }
    }
}
