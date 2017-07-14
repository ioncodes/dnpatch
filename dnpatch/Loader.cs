using System.Collections.Generic;

namespace dnpatch
{
    public class Loader
    {
        public Dictionary<string, Assembly> Assemblies { get; internal set; } // Set of all loaded assemblies for patching

        public Loader() 
        {
            Assemblies = new Dictionary<string, Assembly>();    
        }

        public void Initialize(string name, string fileName, bool overwriteOriginal) 
        {
            Assemblies.Add(name, new Assembly(new AssemblyInfo()
            {
                Name = fileName,
                OverwriteOriginal = overwriteOriginal
            }));
        }

        public Assembly LoadAssembly(string name)
        {
            return Assemblies[name];
        }
    }
}
