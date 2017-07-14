using System;
using dnlib.DotNet;

namespace dnpatch
{
    public struct AssemblyData
    {
        public ModuleDefMD Module { get; internal set; }
    }
}
