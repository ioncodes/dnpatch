using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace dnpatch
{
    public class Patcher
    {
        private PatchHelper patcher = null;

        public enum MemberRefType
        {
            Static,
            Instance
        }

        public Patcher(string file)
        {
            patcher = new PatchHelper(file);
        }

        public Patcher(string file, bool keepOldMaxStack)
        {
            patcher = new PatchHelper(file, keepOldMaxStack);
        }

        public Patcher(ModuleDefMD module, bool keepOldMaxStack)
        {
            patcher = new PatchHelper(module, keepOldMaxStack);
        }

        public Patcher(Stream stream, bool keepOldMaxStacks)
        {
            patcher = new PatchHelper(stream, keepOldMaxStacks);
        }

        public void Patch(Target target)
        {
            if ((target.Indices != null || target.Index != -1) &&
                (target.Instruction != null || target.Instructions != null))
            {
                patcher.PatchOffsets(target);
            }
            else if ((target.Index == -1 && target.Indices == null) &&
                     (target.Instruction != null || target.Instructions != null))
            {
                patcher.PatchAndClear(target);
            }
            else
            {
                throw new Exception("Check your Target object for inconsistent assignements");
            }
        }

        public void Patch(Target[] targets)
        {
            foreach (Target target in targets)
            {
                if ((target.Indices != null || target.Index != -1) &&
                    (target.Instruction != null || target.Instructions != null))
                {
                    patcher.PatchOffsets(target);
                }
                else if ((target.Index == -1 && target.Indices == null) &&
                         (target.Instruction != null || target.Instructions != null))
                {
                    patcher.PatchAndClear(target);
                }
                else
                {
                    throw new Exception("Check your Target object for inconsistent assignements");
                }
            }
        }

        public void Save(string name)
        {
            patcher.Save(name);
        }

        public void Save(bool backup)
        {
           patcher.Save(backup);
        }

        public int FindInstruction(Target target, Instruction instruction)
        {
            return patcher.FindInstruction(target, instruction, 1);
        }

        public int FindInstruction(Target target, Instruction instruction, int occurence)
        {
            return patcher.FindInstruction(target, instruction, occurence);
        }

        public void ReplaceInstruction(Target target)
        {
            patcher.ReplaceInstruction(target);
        }

        public void RemoveInstruction(Target target)
        {
            patcher.RemoveInstruction(target);
        }

        public Instruction[] GetInstructions(Target target)
        {
            return patcher.GetInstructions(target);
        }

        public void PatchOperand(Target target, string operand)
        {
            patcher.PatchOperand(target, operand);
        }

        public void PatchOperand(Target target, int operand)
        {
            patcher.PatchOperand(target, operand);
        }

        public void PatchOperand(Target target, string[] operand)
        {
            patcher.PatchOperand(target, operand);
        }

        public void PatchOperand(Target target, int[] operand)
        {
            patcher.PatchOperand(target, operand);
        }

        public void WriteReturnBody(Target target, bool trueOrFalse)
        {
            target = patcher.FixTarget(target);
            if (trueOrFalse)
            {
                target.Instructions = new Instruction[]
                {
                    Instruction.Create(OpCodes.Ldc_I4_1),
                    Instruction.Create(OpCodes.Ret)
                };
            }
            else
            {
                target.Instructions = new Instruction[]
                {
                    Instruction.Create(OpCodes.Ldc_I4_0),
                    Instruction.Create(OpCodes.Ret)
                };
            }

            patcher.PatchAndClear(target);
        }

        /// <summary>
        /// Find methods that contain a certain OpCode[] signature
        /// </summary>
        /// <param name="signature"></param>
        /// <returns></returns>
        public HashSet<MethodDef> FindMethodsByOpCodeSignature(OpCode[] signature)
        {
            return patcher.FindMethodsByOpCodeSignature(signature);
        }

        public void WriteEmptyBody(Target target)
        {
            target = patcher.FixTarget(target);
            target.Instruction = Instruction.Create(OpCodes.Ret);
            patcher.PatchAndClear(target);
        }

        public Target[] FindInstructionsByOperand(string[] operand)
        {
            return patcher.FindInstructionsByOperand(operand);
        }

        public Target[] FindInstructionsByOperand(int[] operand)
        {
            return patcher.FindInstructionsByOperand(operand);
        }

        public Target[] FindInstructionsByOpcode(OpCode[] opcode)
        {
            return patcher.FindInstructionsByOpcode(opcode);
        }

        public Target[] FindInstructionsByOperand(Target target, int[] operand, bool removeIfFound = false)
        {
            return patcher.FindInstructionsByOperand(target, operand, removeIfFound);
        }

        public Target[] FindInstructionsByOpcode(Target target, OpCode[] opcode, bool removeIfFound = false)
        {
            return patcher.FindInstructionsByOpcode(target, opcode, removeIfFound);
        }

        public string GetOperand(Target target)
        {
            return patcher.GetOperand(target);
        }

        public MemberRef BuildMemberRef(string ns, string cs, string name, MemberRefType type)
        {
            return patcher.BuildMemberRef(ns, cs, name, type);
        }
    }
}
