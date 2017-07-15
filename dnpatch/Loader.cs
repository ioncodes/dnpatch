using System.Collections.Generic;
using System.IO;

namespace dnpatch
{
    public class Loader
    {
        public Dictionary<string, Assembly> Assemblies { get; internal set; } // Set of all loaded assemblies for patching

        public Loader() 
        {
            Assemblies = new Dictionary<string, Assembly>();    
        }

        public void Initialize(string name, string fileName, bool overwriteOriginal, bool createBackup) 
        {
            Assemblies.Add(name, new Assembly(new AssemblyInfo()
            {
                Name = fileName,
                OverwriteOriginal = overwriteOriginal,
                InternalName = name,
                CreateBackup = createBackup
            }));
        }

        public Assembly LoadAssembly(string name)
        {
            return Assemblies.TryGetValue(name, out Assembly value) ? value : null;
        }

        public void Write()
        {
            foreach(var assembly in Assemblies)
            {
                var asm = assembly.Value; // no not ASMR, ffs...
                if(asm.AssemblyInfo.CreateBackup)
                {
                    File.Copy(asm.AssemblyInfo.Name, $"{asm.AssemblyInfo.Name}.bak");
                }
                if (asm.AssemblyInfo.OverwriteOriginal)
                {
                    asm.AssemblyData.Module.Write($"{asm.AssemblyInfo.OutputName}.tmp");
                    asm.AssemblyData.Module.Dispose();
                    File.Delete(asm.AssemblyInfo.OutputName);
                    File.Move($"{asm.AssemblyInfo.OutputName}.tmp", asm.AssemblyInfo.OutputName);
                    continue;
                }
                asm.AssemblyData.Module.Write(asm.AssemblyInfo.OutputName);
            }
        }
    }
}
