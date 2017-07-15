using System;
namespace dnpatch
{
    public struct AssemblyInfo
    {
        public bool OverwriteOriginal { get; internal set; }
        public string Name { get; internal set; }
        public string InternalName { get; internal set; }

        public override string ToString()
        {
            return $"[AssemblyInfo: OverwriteOriginal={OverwriteOriginal}, Name={Name}]";
        }
    }
}
