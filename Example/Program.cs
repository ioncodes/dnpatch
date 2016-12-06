using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnpatch;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Replaces all instructions with your own body
             */
            Patcher p = new Patcher("Test.exe");
            Instruction[] opcodesConsoleWriteLine = {
                Instruction.Create(OpCodes.Ldstr, "Hello Sir"), // String to print
                Instruction.Create(OpCodes.Call, p.BuildMemberRef("System", "Console", "WriteLine", Patcher.MemberRefType.Static)), // Console.WriteLine call
                Instruction.Create(OpCodes.Ret) // Alaway return smth
            };
            Target target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "Print",
                Instructions = opcodesConsoleWriteLine
            };
            p.Patch(target);
            p.Save("Test1.exe");


            /*
             * Replaces the instructions at the given index
             */
            p = new Patcher("Test.exe");
            Instruction[] opCodesManipulateOffset = {
                Instruction.Create(OpCodes.Ldstr, "Place easter egg here 1"),
                Instruction.Create(OpCodes.Ldstr, "Place easter egg here 2")
            };
            int[] indexes = {
                4,
                8
            };
            target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "PrintAlot",
                Instructions = opCodesManipulateOffset,
                Indexes = indexes
            };
            p.Patch(target);
            p.Save("Test2.exe");


            /*
             * Replaces the instructions at the given index in a nested class
             */
            p = new Patcher("Test.exe");
            Instruction opCodeManipulateOffsetNestedClass = Instruction.Create(OpCodes.Ldstr, "FooBarCode");
            int index = 0;
            string nestedClass = "Bar";
            target = new Target()
            {
                Namespace = "Test",
                Class = "Foo",
                NestedClass = nestedClass,
                Method = "NestedPrint",
                Instruction = opCodeManipulateOffsetNestedClass,
                Index = index
            };
            p.Patch(target);
            p.Save("Test3.exe");


            /*
             * Replaces the instructions at the given index in a big nested class
             */
            p = new Patcher("Test.exe");
            Instruction opCodeManipulateOffsetNestedClasses = Instruction.Create(OpCodes.Ldstr, "Eat fruits");
            index = 0;
            string[] nestedClasses = {
                "Am",
                "A",
                "Burger"
            };
            target = new Target()
            {
                Namespace = "Test",
                Class = "I",
                NestedClasses = nestedClasses,
                Method = "Eat",
                Instruction = opCodeManipulateOffsetNestedClasses,
                Index = index
            };
            p.Patch(target);
            p.Save("Test4.exe");


            /*
             * Replaces the instructions at the given index which has been find via FindInstruction
             */
            p = new Patcher("Test.exe");
            Instruction opCodeReplaceInstruction = Instruction.Create(OpCodes.Ldstr, "TheTrain");
            Instruction toFind = Instruction.Create(OpCodes.Ldstr, "TheWord");
            target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "FindMe",
                Instruction = opCodeReplaceInstruction
            };
            target.Index = p.FindInstruction(target, toFind);
            p.Patch(target);
            p.Save("Test5.exe");


            /*
             * Replaces a instruction at the given index
             */
            p = new Patcher("Test.exe");
            Instruction opCodeReplaceMe = Instruction.Create(OpCodes.Ldstr, "I love kittens");
            target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "ReplaceMe",
                Instruction = opCodeReplaceMe,
                Index = 0
            };
            p.ReplaceInstruction(target);
            p.Save("Test6.exe");


            /*
             * Removes the instrutions at the given indexes
             */
            p = new Patcher("Test.exe");
            target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "RemoveMe",
                Indexes = new[]{0,1}
            };
            p.RemoveInstruction(target);
            p.Save("Test7.exe");


            /*
             * Patches the operands at the given index
             */
            p = new Patcher("Test.exe");
            target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "PrintAlot",
                Index = 0
            };
            p.PatchOperand(target, "PatchedOperand");
            p.Save("Test8.exe");


            /*
             * Tries to find indexes in a obfuscated assembly by string operands
             */
            var op = new Patcher("TestObfuscated.exe", true);
            string[] operands = {
                "Find",
                "TheWord",
                "The",
                "Word",
                "You",
                "Wont"
            };
            var obfuscatedTargets = op.FindInstructionsByOperand(operands);
            foreach (var obfTarget in obfuscatedTargets)
            {
                obfTarget.Instructions = new Instruction[]
                {
                    Instruction.Create(OpCodes.Ldstr, "Obfuscator"),
                    Instruction.Create(OpCodes.Ldstr, "Got"),
                    Instruction.Create(OpCodes.Ldstr, "Rekt"),
                    Instruction.Create(OpCodes.Ldstr, "Hell"),
                    Instruction.Create(OpCodes.Ldstr, "Yeah"),
                    Instruction.Create(OpCodes.Ldstr, "!")
                };
            }
            op.Patch(obfuscatedTargets);
            op.Save("TestObfuscated1.exe");


            /*
             * Overwrites methodbody with return true
             */
            target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "VerifyMe"
            };
            p.WriteReturnBody(target, true);
            p.Save("Test9.exe");


            /*
             * Overwrites methodbody with return false
             */
            target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "VerifyMeNot"
            };
            p.WriteReturnBody(target, false);
            p.Save("Test10.exe");


            /*
             * Clears the methodbody
             */
            target = new Target()
            {
                Namespace = "Test",
                Class = "Program",
                Method = "WriteLog"
            };
            p.WriteEmptyBody(target);
            p.Save("Test11.exe");


            /*
             * Find method in obfuscated assembly by OpCodes
             */
            op = new Patcher("TestObfuscated.exe", true);
            OpCode[] opc = { 
                OpCodes.Ldstr,
                OpCodes.Call,
                OpCodes.Call,
                OpCodes.Call,
                OpCodes.Call,
                OpCodes.Brfalse_S,
                OpCodes.Stloc_0,
                OpCodes.Ldc_I4_0,
                OpCodes.Br_S,
                OpCodes.Ldloc_1,
                OpCodes.Add,
                OpCodes.Blt_S
            }; // find these
            obfuscatedTargets = op.FindInstructionsByOpcode(opc);
            foreach (var obfTarget in obfuscatedTargets)
            {
                obfTarget.Instructions = new Instruction[]
                {
                    Instruction.Create(OpCodes.Ldstr, "Obfuscators cant beat my library :P"),
                    Instruction.Create(OpCodes.Call, op.BuildMemberRef("System", "Console", "WriteLine", Patcher.MemberRefType.Static)),
                    Instruction.Create(OpCodes.Ret)  
                };
                obfTarget.Indexes = null; // Replace whole body
            }
            op.Patch(obfuscatedTargets);
            op.Save("TestObfuscated2.exe");

            Console.Read();
        }
    }
}