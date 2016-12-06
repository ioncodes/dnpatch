using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet.Emit;
using dnpatch;
using dnpatch.deobfuscation;

namespace UnpackMe2
{
    class Program
    {
        /*
         * First field: ioncodes
         * Click Button 1
         * Second field: ionCODES
         * Click Button 2
         * Enjoy Patches MessageBox
         */
        static void Main(string[] args)
        {
            Deobfuscation deobfuscation = new Deobfuscation("UnpackMe2.ori.exe", "UnpackMe2.deob1.exe");
            deobfuscation.Deobfuscate();
            deobfuscation = new Deobfuscation("UnpackMe2.deob1.exe", "UnpackMe2.deob2.exe");
            deobfuscation.Deobfuscate();

            Patcher obfuscationPatcher = new Patcher("UnpackMe2.deob2.exe", false);
            Target[] targets = obfuscationPatcher.FindInstructionsByOperand(new[]
            {
                "bBbBbBbBb",
                "AaAaA",
                "Good"
            });
            foreach (var target in targets)
            {
                target.Instructions = new[]
                {
                    Instruction.Create(OpCodes.Ldstr, "ion"),
                    Instruction.Create(OpCodes.Ldstr, "codes"),
                    Instruction.Create(OpCodes.Ldstr, "Patched!")
                };
            }
            obfuscationPatcher.Patch(targets);

            targets = obfuscationPatcher.FindInstructionsByOperand(new []
            {
                "ameereagle"
            });
            foreach (var target in targets)
            {
                target.Instruction = Instruction.Create(OpCodes.Ldstr, "ioncodes");
            }
            obfuscationPatcher.Patch(targets);

            obfuscationPatcher.Save("UnpackMe2.patched.exe");
        }
    }
}
