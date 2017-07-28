using dnlib.DotNet;

namespace dnpatch.Structs
{
    public struct AssemblyData
    {
        public ModuleDefMD Module { get; internal set; }
        public MethodDef Entrypoint { get; internal set; }
        public Importer Importer { get; internal set; }
    }
}
