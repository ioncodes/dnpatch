using dnlib.DotNet;
using dnpatch.Misc;
using dnpatch.Provider;
using dnpatch.Structs;

namespace dnpatch.Types
{
    /// <summary>
    /// The assembly
    /// </summary>
    public class Assembly
    {
        /// <summary>
        /// The assembly information
        /// </summary>
        public AssemblyInfo AssemblyInfo;
        /// <summary>
        /// The assembly data
        /// </summary>
        public AssemblyData AssemblyData;
        /// <summary>
        /// The assembly model
        /// </summary>
        public AssemblyModel AssemblyModel;
        /// <summary>
        /// IL processing accessor
        /// </summary>
        public Context.Assembly.ILContext IL;
        /// <summary>
        /// Instruction processing accessor
        /// </summary>
        public Context.Assembly.InstructionContext Instructions;
        /// <summary>
        /// Model accessor
        /// </summary>
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
