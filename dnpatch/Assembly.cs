using System;
namespace dnpatch
{
    public class Assembly
    {
        public AssemblyInfo AssemblyInfo;

        internal Assembly(AssemblyInfo assemblyInfo)
        {
            AssemblyInfo = assemblyInfo;
        }
    }
}
