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
        public enum MemberRefType
        {
            Static,
            Instance
        }

        public Patcher(string file)
        {
            PatchHelper.Module = ModuleDefMD.Load(file);
            PatchHelper.OFile = file;
        }

        public Patcher(string file, bool keepOldMaxStack)
        {
            PatchHelper.Module = ModuleDefMD.Load(file);
            PatchHelper.OFile = file;
            PatchHelper.KeepOldMaxStack = keepOldMaxStack;
        }

        public void Patch(Target target)
        {
            if ((target.Indexes != null || target.Index != -1) &&
                (target.Instruction != null || target.Instructions != null))
            {
                PatchHelper.PatchOffsets(target);
            }
            else if ((target.Index == -1 && target.Indexes == null) &&
                     (target.Instruction != null || target.Instructions != null))
            {
                PatchHelper.PatchAndClear(target);
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
                if ((target.Indexes != null || target.Index != -1) &&
                    (target.Instruction != null || target.Instructions != null))
                {
                    PatchHelper.PatchOffsets(target);
                }
                else if ((target.Index == -1 && target.Indexes == null) &&
                         (target.Instruction != null || target.Instructions != null))
                {
                    PatchHelper.PatchAndClear(target);
                }
                else
                {
                    throw new Exception("Check your Target object for inconsistent assignements");
                }
            }
        }

        public void Save(string name)
        {
            PatchHelper.Save(name);
        }

        public void Save(bool backup)
        {
           PatchHelper.Save(backup);
        }

        public int FindInstruction(Target target, Instruction instruction)
        {
            return PatchHelper.FindInstruction(target, instruction, 1);
        }

        public int FindInstruction(Target target, Instruction instruction, int occurence)
        {
            return PatchHelper.FindInstruction(target, instruction, occurence);
        }

        public void ReplaceInstruction(Target target)
        {
            PatchHelper.ReplaceInstruction(target);
        }

        public void RemoveInstruction(Target target)
        {
            PatchHelper.RemoveInstruction(target);
        }

        public Instruction[] GetInstructions(Target target)
        {
            return PatchHelper.GetInstructions(target);
        }

        public void PatchOperand(Target target, string operand)
        {
            PatchHelper.PatchOperand(target, operand);
        }

        public void PatchOperand(Target target, int operand)
        {
            PatchHelper.PatchOperand(target, operand);
        }

        public void PatchOperand(Target target, string[] operand)
        {
            PatchHelper.PatchOperand(target, operand);
        }

        public void PatchOperand(Target target, int[] operand)
        {
            PatchHelper.PatchOperand(target, operand);
        }

        public void WriteReturnBody(Target target, bool trueOrFalse)
        {
            target = PatchHelper.FixTarget(target);
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

            PatchHelper.PatchAndClear(target);
        }

        public void WriteEmptyBody(Target target)
        {
            target = PatchHelper.FixTarget(target);
            target.Instruction = Instruction.Create(OpCodes.Ret);
            PatchHelper.PatchAndClear(target);
        }

        public Target[] FindInstructionsByOperand(string[] operand)
        {
            return PatchHelper.FindInstructionsByOperand(operand);
        }

        public Target[] FindInstructionsByOperand(int[] operand)
        {
            return PatchHelper.FindInstructionsByOperand(operand);
        }

        public Target[] FindInstructionsByOpcode(OpCode[] opcode)
        {
            return PatchHelper.FindInstructionsByOpcode(opcode);
        }

        public Target[] FindInstructionsByOperand(Target target, int[] operand, bool removeIfFound = false)
        {
            return PatchHelper.FindInstructionsByOperand(target, operand, removeIfFound);
        }

        public Target[] FindInstructionsByOpcode(Target target, OpCode[] opcode, bool removeIfFound = false)
        {
            return PatchHelper.FindInstructionsByOpcode(target, opcode, removeIfFound);
        }

        public string GetOperand(Target target)
        {
            return PatchHelper.GetOperand(target);
        }

        public MemberRef BuildMemberRef(string ns, string cs, string name, MemberRefType type)
        {
            return PatchHelper.BuildMemberRef(ns, cs, name, type);
        }
    }
}
