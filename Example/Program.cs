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

            loader.Initialize("crack", "Security.dll", true, true); // crack the license mechanism
            loader.Initialize("credits", "UI.dll", true, true); // add credits to the UI window

            Assembly security = loader.LoadAssembly("crack");
            Assembly ui = loader.LoadAssembly("credits");

            Console.WriteLine(security.AssemblyInfo.ToString());
            Console.WriteLine(ui.AssemblyInfo.ToString());

            security.SetNamespace("Security");
            security.SetType("Security");
            security.SetMethod("IsLicensed");

            ui.SetNamespace("UI");
            ui.SetType("UI");
            ui.SetMethod("GetCredits");

            Console.Read();
        }
    }
}