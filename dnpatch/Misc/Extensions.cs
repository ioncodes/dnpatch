using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
