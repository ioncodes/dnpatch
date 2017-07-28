using dnpatch.Types;

namespace dnpatch.Provider
{
    internal static class ContextProvider
    {
        internal static void Provide(Assembly assembly)
        {
            assembly.IL = new Context.Assembly.ILContext(assembly);
            assembly.Model = new Context.Assembly.ModelContext(assembly);
            assembly.Instructions = new Context.Assembly.InstructionContext(assembly);
        }
    }
}
