using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnpatch.Enums;
using dnpatch.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dnpatch.tests
{
    [TestClass()]
    public class FindTests
    {
        [TestMethod()]
        public void FindDefault()
        {
            Loader loader = new Loader();

            loader.Initialize("crack", "Security.dll", "Security.default.dll", false, true, true);

            Assembly security = loader.LoadAssembly("crack");

            var method = security.IL.FindMethod(new Instruction[]
            {
                Instruction.Create(OpCodes.Ldstr, "Not Licensed!"),
                Instruction.Create(OpCodes.Stloc_0), 
            }, SearchMode.Default)[0];

            Assert.IsNotNull(method);
            Assert.IsTrue(method.Name == "IsLicensed");
        }

        [TestMethod()]
        public void FindConsecutive()
        {
            Loader loader = new Loader();

            loader.Initialize("crack", "Security.dll", "Security.default.dll", false, true, true);

            Assembly security = loader.LoadAssembly("crack");

            var method = security.IL.FindMethod(new Instruction[]
            {
                Instruction.Create(OpCodes.Nop), 
                Instruction.Create(OpCodes.Ldstr, "Not Licensed!"),
            }, SearchMode.Consecutive)[0];

            Assert.IsNotNull(method);
            Assert.IsTrue(method.Name == "IsLicensed");
        }

        [TestMethod()]
        public void FindOpCodeDefault()
        {
            Loader loader = new Loader();

            loader.Initialize("crack", "Security.dll", "Security.default.dll", false, true, true);

            Assembly security = loader.LoadAssembly("crack");

            var method = security.IL.FindMethod(new OpCode[]
            {
                OpCodes.Ldstr,
                OpCodes.Stloc_0
            }, SearchMode.Default)[0];

            Assert.IsNotNull(method);
            Assert.IsTrue(method.Name == "IsLicensed");
        }

        [TestMethod()]
        public void FindOpCodeConsecutive()
        {
            Loader loader = new Loader();

            loader.Initialize("crack", "Security.dll", "Security.default.dll", false, true, true);

            Assembly security = loader.LoadAssembly("crack");

            var method = security.IL.FindMethod(new OpCode[]
            {
                OpCodes.Nop,
                OpCodes.Ldstr
            }, SearchMode.Consecutive)[0];

            Assert.IsNotNull(method);
            Assert.IsTrue(method.Name == "IsLicensed");
        }
    }
}