using System;
using dnlib.DotNet.Emit;
using dnpatch;
using dnpatch.Types;

namespace Example1
{
    class MainClass
    {
        // Set model by Type & MethodInfo reference
        public static void Main(string[] args)
        {
			Loader loader = new Loader();

			loader.Initialize("crack", "Security.dll", "Security.dll", true, true); // crack the license mechanism
			loader.Initialize("credits", "UI.dll", "UI.dll", true, true); // add credits to the UI window

			Assembly security = loader.LoadAssembly("crack");
			Assembly ui = loader.LoadAssembly("credits");

			Console.WriteLine(security.AssemblyInfo.ToString());
			Console.WriteLine(ui.AssemblyInfo.ToString());

			security.Model.SetNamespace("Security");
            security.Model.SetType(typeof(Security.Security));
            security.Model.SetMethod(typeof(Security.Security).GetMethod("IsLicensed"));

			ui.Model.SetNamespace("UI");
			ui.Model.SetType(typeof(UI.UI));
			ui.Model.SetMethod(typeof(UI.UI).GetMethod("GetCredits"));

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
