using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnpatch.script;

namespace TestScript
{
    class Program
    {
        static void Main(string[] args)
        {
            Script script = new Script("script.json");
            script.Patch();
            script.Save("scripted.exe");
        }
    }
}
