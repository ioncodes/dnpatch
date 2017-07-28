namespace dnpatch.Structs
{
    /// <summary>
    /// The assembly settings
    /// </summary>
    public struct AssemblyInfo
    {
        /// <summary>
        /// Gets a value indicating whether overwrite the original assembly or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if overwrite original assembly; otherwise, <c>false</c>.
        /// </value>
        public bool OverwriteOriginal { get; internal set; }
        /// <summary>
        /// Gets a value indicating whether create a backup or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if create backup; otherwise, <c>false</c>.
        /// </value>
        public bool CreateBackup { get; internal set; }
        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>
        /// The original filename
        /// </value>
        public string Name { get; internal set; }
        /// <summary>
        /// Gets the name of the output file.
        /// </summary>
        /// <value>
        /// The name of the outputfile.
        /// </value>
        public string OutputName { get; internal set; }
        /// <summary>
        /// Gets the internal identifier.
        /// </summary>
        /// <value>
        /// The internal identifier.
        /// </value>
        public string InternalName { get; internal set; }
        /// <summary>
        /// Gets a value indicating whether preload types and methods.
        /// </summary>
        /// <value>
        ///   <c>true</c> if preload data; otherwise, <c>false</c>.
        /// </value>
        public bool PreloadData { get; internal set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[AssemblyInfo: OverwriteOriginal={OverwriteOriginal}, CreateBackup={CreateBackup}, Name={Name}, OutputName={OutputName}, InternalName={InternalName}]";
        }
    }
}
