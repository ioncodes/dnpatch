using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnpatch;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Loader loader = new Loader();

            loader.Initialize("crack", "Security.dll", true); // crack the license mechanism
            loader.Initialize("credits", "UI.dll", true); // add credits to the UI window

            Assembly license = loader.LoadAssembly("crack");
            Assembly ui = loader.LoadAssembly("credits");

            Console.WriteLine(license.AssemblyInfo.ToString());
            Console.WriteLine(ui.AssemblyInfo.ToString());

            Console.Read();
        }
    }
}