using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet.Emit;
using dnpatch;
using dnpatch.deobfuscation;

namespace UnpackMe1
{
    class Program
    {
        static void Main(string[] args)
        {
            const string file = "UnpackMe_Confuser 1.7.exe";
            const string newFile = "UnpackMe_Confuser 1.7.deob.exe";
            Deobfuscation deobfuscation = new Deobfuscation(file, newFile);
            deobfuscation.Deobfuscate();

            Patcher patcher = new Patcher(newFile);
            Target target = new Target()
            {
                Class = "Form1",
                Namespace = "Confuser_1._7",
                Method = "Form1_Load",
                Indices = new []
                {
                    0,
                    1,
                    2,
                    3,
                    4,
                    5,
                    6
                }
            };
            patcher.RemoveInstruction(target);
            target.Indices = null;
            target.Index = 2;
            patcher.PatchOperand(target, "Patched!");
            patcher.Save("UnpackMe_Confuser 1.7.deob.patch.exe");
        }
    }
}
