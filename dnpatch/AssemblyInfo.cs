using System;
namespace dnpatch
{
    public struct AssemblyInfo
    {
        public bool OverwriteOriginal { get; internal set; }
        public string Name { get; internal set; }
    }
}
