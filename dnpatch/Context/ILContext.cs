namespace dnpatch
{
    public partial class Assembly
    {
        public class ILContext : ILProcessor
        {
            internal ILContext(Assembly assembly) : base(assembly)
            {
                _assembly = assembly;
            }
        }
    }
}
