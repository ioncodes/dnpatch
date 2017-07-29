using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnpatch.Types;

namespace dnpatch.Misc
{
    internal static class Extensions
    {
        public static bool ContainsSequence<T>(this IList<T> outer, IList<T> inner)
        {
            var innerCount = inner.Count;

            for (int i = 0; i < outer.Count - innerCount; i++)
            {
                bool isMatch = true;
                for (int x = 0; x < innerCount; x++)
                {
                    if (outer[i + x].Equals(inner[x])) continue;
                    isMatch = false;
                    break;
                }

                if (isMatch) return true;
            }

            return false;
        }

        public static List<MethodDef> GetAllMethods(this Assembly assembly)
        {
            List<MethodDef> methods = new List<MethodDef>();
            foreach (var type in assembly.AssemblyData.Module.Types)
            {
                methods.AddRange(type.Methods.Where(i => i.HasBody));
            }
            return methods;
        }

        public static List<TypeDef> GetAllTypes(this Assembly assembly)
        {
            return assembly.AssemblyData.Module.Types as List<TypeDef>;
        }
    }
}
