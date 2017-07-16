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
        public ModelContext Model;

        internal Assembly(AssemblyInfo assemblyInfo)
        {
            AssemblyInfo = assemblyInfo;
            AssemblyData = new AssemblyData() // Load assembly data
            {
                Module = ModuleDefMD.Load(AssemblyInfo.Name)
            };
            AssemblyModel = new AssemblyModel();
            ContextProvider.Provide(this);
        }
    }
}
