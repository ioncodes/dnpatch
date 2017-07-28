using dnlib.DotNet;
using dnpatch.Misc;
using dnpatch.Provider;
using dnpatch.Structs;

namespace dnpatch.Types
{
    public partial class Assembly
    {
        public AssemblyInfo AssemblyInfo;
        public AssemblyData AssemblyData;
        public AssemblyModel AssemblyModel;
        public Context.Assembly.ILContext IL;
        public Context.Assembly.InstructionContext Instructions;
        public Context.Assembly.ModelContext Model;

        internal Assembly(AssemblyInfo assemblyInfo)
        {
            AssemblyInfo = assemblyInfo;
            var module = ModuleDefMD.Load(AssemblyInfo.Name);
            AssemblyData = new AssemblyData() // Load assembly data
            {
                Module = module,
                Entrypoint = module.IsEntryPointValid ? module.EntryPoint : null,
                Importer = new Importer(module)
            };
            if (assemblyInfo.PreloadData)
            {
                AssemblyData.Types = this.GetAllTypes();
                AssemblyData.Methods = this.GetAllMethods();
            }
            AssemblyModel = new AssemblyModel();
            ContextProvider.Provide(this);
        }
    }
}
