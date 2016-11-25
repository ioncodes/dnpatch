using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using de4dot.code;
using de4dot.code.AssemblyClient;
using de4dot.code.deobfuscators;
using de4dot.cui;
using dnlib.DotNet;

namespace dnpatch.deobfuscation
{
    /*
     * Credits go to 0xd4d for de4dot!
     */

    public class Deobfuscation
    {
        private string file;
        private string newFile;

        public Deobfuscation(string file, string newFile)
        {
            this.file = file;
            this.newFile = newFile;
        }

        public void Deobfuscate() // ALPHA TEST
        {
            FilesDeobfuscator.Options filesOptions = new FilesDeobfuscator.Options();
            filesOptions.ControlFlowDeobfuscation = true;
            new CommandLineParser(CreateDeobfuscatorInfos(), filesOptions).Parse(new []
            {
                file,
                "-o",
                newFile
            });
            new FilesDeobfuscator(filesOptions).DoIt();
        }

        static IList<IDeobfuscatorInfo> CreateDeobfuscatorInfos()
        {
            var local = new List<IDeobfuscatorInfo> {
                new de4dot.code.deobfuscators.Unknown.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Agile_NET.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Babel_NET.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.CodeFort.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.CodeVeil.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.CodeWall.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Confuser.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.CryptoObfuscator.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.DeepSea.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Dotfuscator.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.dotNET_Reactor.v3.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.dotNET_Reactor.v4.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Eazfuscator_NET.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Goliath_NET.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.ILProtector.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.MaxtoCode.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.MPRESS.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Rummage.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Skater_NET.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.SmartAssembly.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Spices_Net.DeobfuscatorInfo(),
                new de4dot.code.deobfuscators.Xenocode.DeobfuscatorInfo(),
            };
            var dict = new Dictionary<string, IDeobfuscatorInfo>();
            foreach (var d in local)
                dict[d.Type] = d;
            return new List<IDeobfuscatorInfo>(dict.Values);
        }
    }
}
