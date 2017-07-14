using System;
namespace dnpatch
{
    public struct AssemblyInfo
    {
        public bool OverwriteOriginal { get; internal set; }
        public string Name { get; internal set; }

        public override string ToString()
        {
            return string.Format("[AssemblyInfo: OverwriteOriginal={0}, Name={1}]", OverwriteOriginal, Name);
        }
    }
}
