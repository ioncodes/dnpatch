namespace dnpatch.Structs
{
    public struct AssemblyInfo
    {
        public bool OverwriteOriginal { get; internal set; }
        public bool CreateBackup { get; internal set; }
        public string Name { get; internal set; }
        public string OutputName { get; internal set; }
        public string InternalName { get; internal set; }

        public override string ToString()
        {
            return $"[AssemblyInfo: OverwriteOriginal={OverwriteOriginal}, CreateBackup={CreateBackup}, Name={Name}, OutputName={OutputName}, InternalName={InternalName}]";
        }
    }
}
