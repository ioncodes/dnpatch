using System;
using dnlib.DotNet.Emit;
using dnpatch;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Loader loader = new Loader();

            loader.Initialize("crack", "Security.dll", "Security.dll", true, true); // crack the license mechanism
            loader.Initialize("credits", "UI.dll", "UI.dll", true, true); // add credits to the UI window

            Assembly security = loader.LoadAssembly("crack");
            Assembly ui = loader.LoadAssembly("credits");

            Console.WriteLine(security.AssemblyInfo.ToString());
            Console.WriteLine(ui.AssemblyInfo.ToString());

            security.Model.SetNamespace("Security");
            security.Model.SetType("Security");
            security.Model.SetMethod("IsLicensed");

            ui.Model.SetNamespace("UI");
            ui.Model.SetType("UI");
            ui.Model.SetMethod("GetCredits");

            security.IL.Overwrite(instructions: new Instruction[] // return true
            {
                Instruction.Create(OpCodes.Ldc_I4_1),
                Instruction.Create(OpCodes.Ret)
            });

            ui.IL.Write(Instruction.Create(OpCodes.Ldstr, "Cracked By Evil-Corp"), 1);

            loader.Save(); // Write changes to disk

            Console.Read();
        }
    }
}