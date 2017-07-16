using System;
using System.Linq;
using System.Collections.Generic;
using dnlib.DotNet;

namespace dnpatch
{
    public partial class Assembly
    {
        public AssemblyInfo AssemblyInfo;
        public AssemblyData AssemblyData;
        public AssemblyModel AssemblyModel;
        public ILContext IL;
        public InstructionContext Instructions;
        public ModelContext Model;

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
            AssemblyModel = new AssemblyModel();
            ContextProvider.Provide(this);
        }
    }
}
