using System.Collections.Generic;
using System.IO;
using dnpatch.Structs;
using dnpatch.Types;

namespace dnpatch
{
    /// <summary>
    /// Assembly loader and manager
    /// </summary>
    public class Loader
    {
        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <value>
        /// The assemblies.
        /// </value>
        public Dictionary<string, Assembly> Assemblies { get; internal set; } // Set of all loaded assemblies for patching

        /// <summary>
        /// Initializes a new instance of the <see cref="Loader"/> class.
        /// </summary>
        public Loader() 
        {
            Assemblies = new Dictionary<string, Assembly>();    
        }

        /// <summary>
        /// Initializes a new assembly
        /// </summary>
        /// <param name="name">Internal identifier for the assembly</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="outputName">Name of the output.</param>
        /// <param name="overwriteOriginal">Overwrite original assembly</param>
        /// <param name="createBackup">Create backup of assembly.</param>
        /// <param name="preloadData">Preload Types and Methods.</param>
        public void Initialize(string name, string fileName, string outputName, bool overwriteOriginal, bool createBackup, bool preloadData) 
        {
            Assemblies.Add(name, new Assembly(new AssemblyInfo()
            {
                Name = fileName,
                OverwriteOriginal = overwriteOriginal,
                InternalName = name,
                OutputName = outputName,
                CreateBackup = createBackup,
                PreloadData = preloadData
            }));
        }

        /// <summary>
        /// Returns the assembly by the specified identifier
        /// </summary>
        /// <param name="name">The internal identifier.</param>
        /// <returns></returns>
        public Assembly LoadAssembly(string name)
        {
            return Assemblies.TryGetValue(name, out Assembly value) ? value : null;
        }

        /// <summary>
        /// Saves all assemblies.
        /// </summary>
        public void Save()
        {
            foreach(var assembly in Assemblies)
            {
                var asm = assembly.Value; // no not ASMR, ffs...
                SaveAssembly(asm);
            }
        }

        /// <summary>
        /// Saves the specified assembly.
        /// </summary>
        /// <param name="name">The internal identifier.</param>
        public void Save(string name)
        {
            SaveAssembly(Assemblies[name]);
        }

        private void SaveAssembly(Assembly asm)
        {
			if (asm.AssemblyInfo.CreateBackup)
			{
				File.Copy(asm.AssemblyInfo.Name, $"{asm.AssemblyInfo.Name}.bak", true);
			}
			if (asm.AssemblyInfo.OverwriteOriginal)
			{
				asm.AssemblyData.Module.Write($"{asm.AssemblyInfo.OutputName}.tmp");
				asm.AssemblyData.Module.Dispose();
				File.Delete(asm.AssemblyInfo.OutputName);
				File.Move($"{asm.AssemblyInfo.OutputName}.tmp", asm.AssemblyInfo.OutputName);
                return;
			}
			asm.AssemblyData.Module.Write(asm.AssemblyInfo.OutputName);
        }
    }
}
