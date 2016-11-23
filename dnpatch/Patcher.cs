using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace dnpatch
{
    public class Patcher
    {
        private string file;
        private readonly ModuleDefMD module;

        public Patcher(string file)
        {
            this.file = file;
            module = ModuleDefMD.Load(file);
        }

        public void Patch(Target target)
        {
            if ((target.Indexes != null || target.Index != -1) && (target.Instruction != null || target.Instructions != null))
            {
                PatchOffsets(target);
            }
            else if((target.Index == -1 && target.Indexes == null) && (target.Instruction != null || target.Instructions != null))
            {
                PatchAndClear(target);
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
                    PatchOffsets(target);
                }
                else if ((target.Index == -1 && target.Indexes == null) &&
                         (target.Instruction != null || target.Instructions != null))
                {
                    PatchAndClear(target);
                }
                else
                {
                    throw new Exception("Check your Target object for inconsistent assignements");
                }
            }
        }

        public void Save(string name)
        {
            module.Write(name);
        }

        public void Save(bool backup)
        {
            module.Write(file + ".tmp");
            module.Dispose();
            if (backup)
            {
                if (File.Exists(file + ".bak"))
                {
                    File.Delete(file + ".bak");
                }
                File.Move(file, file + ".bak");
            }
            else
            {
                File.Delete(file);
            }
            File.Move(file + ".tmp", file);
        }

        public int FindInstruction(Target target, Instruction instruction)
        {
            var type = FindType(module.Assembly, target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            int index = 0;
            foreach (var i in instructions)
            {
                if (i.OpCode.Name == instruction.OpCode.Name && i.Operand.ToString() == instruction.Operand.ToString())
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public int FindInstruction(Target target, Instruction instruction, int occurence)
        {
            var type = FindType(module.Assembly, target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            int index = 0;
            int occurenceCounter = 0;
            foreach (var i in instructions)
            {
                if (i.OpCode.Name == instruction.OpCode.Name && i.Operand.ToString() == instruction.Operand.ToString() && occurenceCounter < occurence)
                {
                    occurenceCounter++;
                }
                else if (i.OpCode.Name == instruction.OpCode.Name && i.Operand.ToString() == instruction.Operand.ToString() && occurenceCounter == occurence)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public void ReplaceInstruction(Target target)
        {
            string[] nestedClasses = { };
            if (target.NestedClasses != null)
            {
                nestedClasses = target.NestedClasses;
            }
            else if (target.NestedClass != null)
            {
                nestedClasses = new[] { target.NestedClass };
            }
            var type = FindType(module.Assembly, target.Namespace + "." + target.Class, nestedClasses);
            var method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            if (target.Index != -1 && target.Instruction != null)
            {
                instructions[target.Index] = target.Instruction;
            }
            else if (target.Indexes != null && target.Instructions != null)
            {
                foreach (var index in target.Indexes)
                {
                    instructions[index] = target.Instructions[index];
                }
            }
            else
            {
                throw new Exception("Target object built wrong");
            }
        }

        public void RemoveInstruction(Target target)
        {
            string[] nestedClasses = { };
            if (target.NestedClasses != null)
            {
                nestedClasses = target.NestedClasses;
            }
            else if (target.NestedClass != null)
            {
                nestedClasses = new[] { target.NestedClass };
            }
            var type = FindType(module.Assembly, target.Namespace + "." + target.Class, nestedClasses);
            var method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            if (target.Index != -1 && target.Indexes == null)
            {
                instructions.RemoveAt(target.Index);
            }
            else if (target.Index == -1 && target.Indexes != null)
            {
                foreach (var index in target.Indexes.OrderByDescending(v => v))
                {
                    instructions.RemoveAt(index);
                }
            }
            else
            {
                throw new Exception("Target object built wrong");
            }
        }

        public Instruction[] GetInstructions(Target target)
        {
            var type = FindType(module.Assembly, target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            return (Instruction[])method.Body.Instructions;
        }

        public void PatchOperand(Target target, string operand)
        {
            TypeDef type = FindType(module.Assembly, target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            if (target.Indexes == null && target.Index != -1)
            {
                instructions[target.Index].Operand = operand;
            }
            else if (target.Indexes != null && target.Index == -1)
            {
                foreach (var index in target.Indexes)
                {
                    instructions[index].Operand = operand;
                }
            }
            else
            {
                throw new Exception("Operand error");
            }
        }

        public void PatchOperand(Target target, int operand)
        {
            TypeDef type = FindType(module.Assembly, target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            if (target.Indexes == null && target.Index != -1)
            {
                instructions[target.Index].Operand = operand;
            }
            else if (target.Indexes != null && target.Index == -1)
            {
                foreach (var index in target.Indexes)
                {
                    instructions[index].Operand = operand;
                }
            }
            else
            {
                throw new Exception("Operand error");
            }
        }

        public void PatchOperand(Target target, string[] operand)
        {
            TypeDef type = FindType(module.Assembly, target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            if (target.Indexes != null && target.Index == -1)
            {
                foreach (var index in target.Indexes)
                {
                    instructions[index].Operand = operand[index];
                }
            }
            else
            {
                throw new Exception("Operand error");
            }
        }

        public void PatchOperand(Target target, int[] operand)
        {
            TypeDef type = FindType(module.Assembly, target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            if (target.Indexes != null && target.Index == -1)
            {
                foreach (var index in target.Indexes)
                {
                    instructions[index].Operand = operand[index];
                }
            }
            else
            {
                throw new Exception("Operand error");
            }
        }

        public void WriteReturnBody(Target target, bool trueOrFalse)
        {
            target.Indexes = new int[] {};
            target.Index = -1;
            target.Instruction = null;
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

            PatchAndClear(target);
        }

        public MemberRef BuildMemberRef(string ns, string cs, string name) // debug stuff
        {
            TypeRef consoleRef = new TypeRefUser(module, ns, cs, module.CorLibTypes.AssemblyRef);
            return new MemberRefUser(module, name,
                        MethodSig.CreateStatic(module.CorLibTypes.Void, module.CorLibTypes.String),
                        consoleRef);
        }

        private void PatchAndClear(Target target)
        {
            string[] nestedClasses = { };
            if (target.NestedClasses != null)
            {
                nestedClasses = target.NestedClasses;
            }
            else if (target.NestedClass != null)
            {
                nestedClasses = new[] { target.NestedClass };
            }
            var type = FindType(module.Assembly, target.Namespace + "." + target.Class, nestedClasses);
            var method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            instructions.Clear();
            for (int i = 0; i < target.Instructions.Length; i++)
            {
                instructions.Insert(i, target.Instructions[i]);
            }
        }

        private void PatchOffsets(Target target)
        {
            string[] nestedClasses = { };
            if (target.NestedClasses != null)
            {
                nestedClasses = target.NestedClasses;
            }
            else if (target.NestedClass != null)
            {
                nestedClasses = new[] {target.NestedClass};
            }
            var type = FindType(module.Assembly, target.Namespace + "." + target.Class, nestedClasses);
            var method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            if (target.Indexes != null && target.Instructions != null)
            {
                for (int i = 0; i < target.Indexes.Length; i++)
                {
                    instructions[target.Indexes[i]] = target.Instructions[i];
                }
            }
            else if (target.Index != -1 && target.Instruction != null)
            {
                instructions[target.Index] = target.Instruction;
            }
            else if(target.Index == -1)
            {
                throw new Exception("No index specified");
            }
            else if (target.Instruction == null)
            {
                throw new Exception("No instruction specified");
            }
            else if (target.Indexes == null)
            {
                throw new Exception("No indexes specified");
            }
            else if (target.Instructions == null)
            {
                throw new Exception("No instructions specified");
            }
        }

        private TypeDef FindType(AssemblyDef asm, string classPath, string[] nestedClasses)
        {
            foreach (var module in asm.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName == classPath)
                    {
                        TypeDef t = null;
                        if (nestedClasses != null && nestedClasses.Length > 0)
                        {
                            foreach (var nc in nestedClasses)
                            {
                                if (t == null)
                                {
                                    if (!type.HasNestedTypes) continue;
                                    foreach (var typeN in type.NestedTypes)
                                    {
                                        if (typeN.Name == nc)
                                        {
                                            t = typeN;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!t.HasNestedTypes) continue;
                                    foreach (var typeN in t.NestedTypes)
                                    {
                                        if (typeN.Name == nc)
                                        {
                                            t = typeN;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            t = type;
                        }
                        return t;
                    }
                }
            }
            return null;
        }

        private MethodDef FindMethod(TypeDef type, string methodName)
        {
            return type.Methods.FirstOrDefault(m => methodName == m.Name);
        }
    }
}
