using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnpatch.deobfuscation;

namespace ExampleDeobfuscation
{
    class Program
    {
        static void Main(string[] args)
        {
            Deobfuscation d = new Deobfuscation("Dotfuscator.exe", "Dotfuscator.deob.exe");
            d.Deobfuscate();

            d = new Deobfuscation("SmartAssembly.exe", "SmartAssembly.deob.exe");
            d.Deobfuscate();

            d = new Deobfuscation("Babel.exe", "Babel.deob.exe");
            d.Deobfuscate();

            d = new Deobfuscation("Eazfuscator.exe", "Eazfuscator.deob.exe");
            d.Deobfuscate();

            d = new Deobfuscation("ILProtector.exe", "ILProtector.deob.exe");
            d.Deobfuscate();

            d = new Deobfuscation("Confuser.exe", "Confuser.deob.exe");
            d.Deobfuscate();

            Console.Read();
        }
    }
}
