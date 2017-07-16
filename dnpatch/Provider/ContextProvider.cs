using static dnpatch.Assembly;

namespace dnpatch
{
    internal static class ContextProvider
    {
        internal static void Provide(Assembly assembly)
        {
            assembly.IL = new ILContext(assembly);
            assembly.Model = new ModelContext(assembly);
            assembly.Instructions = new InstructionContext(assembly);
        }
    }
}
