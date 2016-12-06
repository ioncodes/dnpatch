using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace dnpatch
{
    internal static class PatchHelper
    {
        public static ModuleDef Module;
        public static string OFile;
        public static bool KeepOldMaxStack = false;

        public static void PatchAndClear(Target target)
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
            var type = FindType(target.Namespace + "." + target.Class, nestedClasses);
            var method = FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            instructions.Clear();
            if (target.Instructions != null)
            {
                for (int i = 0; i < target.Instructions.Length; i++)
                {
                    instructions.Insert(i, target.Instructions[i]);
                }
            }
            else
            {
                instructions.Insert(0, target.Instruction);
            }
        }

        public static void PatchOffsets(Target target)
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
            var type = FindType(target.Namespace + "." + target.Class, nestedClasses);
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
            else if (target.Index == -1)
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

        public static TypeDef FindType(string classPath, string[] nestedClasses)
        {
            foreach (var module in Module.Assembly.Modules)
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

        public static MethodDef FindMethod(TypeDef type, string methodName)
        {
            return type.Methods.FirstOrDefault(m => methodName == m.Name);
        }

        public static Target FixTarget(Target target)
        {
            target.Indexes = new int[] { };
            target.Index = -1;
            target.Instruction = null;
            return target;
        }

        public static void Save(string name)
        {
            if (KeepOldMaxStack)
                Module.Write(name, new ModuleWriterOptions(Module)
                {
                    MetaDataOptions = {Flags = MetaDataFlags.KeepOldMaxStack}
                });
            else
                Module.Write(name);
        }

        public static void Save(bool backup)
        {
            if (KeepOldMaxStack)
                Module.Write(OFile + ".tmp", new ModuleWriterOptions(Module)
                {
                    MetaDataOptions = { Flags = MetaDataFlags.KeepOldMaxStack }
                });
            else
                Module.Write(OFile + ".tmp");
            Module.Dispose();
            if (backup)
            {
                if (File.Exists(OFile + ".bak"))
                {
                    File.Delete(OFile + ".bak");
                }
                File.Move(OFile, OFile + ".bak");
            }
            else
            {
                File.Delete(OFile);
            }
            File.Move(OFile + ".tmp", OFile);
        }

        public static Target[] FindInstructionsByOperand(string[] operand)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<string> operands = operand.ToList();
            foreach (var type in Module.Types)
            {
                if (!type.HasNestedTypes)
                {
                    foreach (var method in type.Methods)
                    {
                        if (method.Body != null)
                        {
                            List<int> indexList = new List<int>();
                            var obfuscatedTarget = new ObfuscatedTarget()
                            {
                                Type = type,
                                Method = method
                            };
                            int i = 0;
                            foreach (var instruction in method.Body.Instructions)
                            {
                                if (instruction.Operand != null)
                                {
                                    if (operands.Contains(instruction.Operand.ToString()))
                                    {
                                        indexList.Add(i);
                                        operands.Remove(instruction.Operand.ToString());
                                    }
                                }
                                i++;
                            }
                            if (indexList.Count == operand.Length)
                            {
                                obfuscatedTarget.Indexes = indexList;
                                obfuscatedTargets.Add(obfuscatedTarget);
                            }
                            operands = operand.ToList();
                        }
                    }
                }
                else
                {
                    var nestedTypes = type.NestedTypes;
                    NestedWorker:
                    foreach (var nestedType in nestedTypes)
                    {
                        foreach (var method in type.Methods)
                        {
                            if (method.Body != null)
                            {
                                List<int> indexList = new List<int>();
                                var obfuscatedTarget = new ObfuscatedTarget()
                                {
                                    Type = type,
                                    Method = method
                                };
                                int i = 0;
                                obfuscatedTarget.NestedTypes.Add(nestedType.Name);
                                foreach (var instruction in method.Body.Instructions)
                                {
                                    if (instruction.Operand != null)
                                    {
                                        if (operands.Contains(instruction.Operand.ToString()))
                                        {
                                            indexList.Add(i);
                                            operands.Remove(instruction.Operand.ToString());
                                        }
                                    }
                                    i++;
                                }
                                if (indexList.Count == operand.Length)
                                {
                                    obfuscatedTarget.Indexes = indexList;
                                    obfuscatedTargets.Add(obfuscatedTarget);
                                }
                                operands = operand.ToList();
                            }
                        }
                        if (nestedType.HasNestedTypes)
                        {
                            nestedTypes = nestedType.NestedTypes;
                            goto NestedWorker;
                        }
                    }
                }
            }
            List<Target> targets = new List<Target>();
            foreach (var obfuscatedTarget in obfuscatedTargets)
            {
                Target t = new Target()
                {
                    Namespace = obfuscatedTarget.Type.Namespace,
                    Class = obfuscatedTarget.Type.Name,
                    Method = obfuscatedTarget.Method.Name,
                    NestedClasses = obfuscatedTarget.NestedTypes.ToArray()
                };
                if (obfuscatedTarget.Indexes.Count == 1)
                {
                    t.Index = obfuscatedTarget.Indexes[0];
                }
                else if (obfuscatedTarget.Indexes.Count > 1)
                {
                    t.Indexes = obfuscatedTarget.Indexes.ToArray();
                }

                targets.Add(t);
            }
            return targets.ToArray();
        }

        public static Target[] FindInstructionsByOperand(int[] operand)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<int> operands = operand.ToList();
            foreach (var type in Module.Types)
            {
                if (!type.HasNestedTypes)
                {
                    foreach (var method in type.Methods)
                    {
                        if (method.Body != null)
                        {
                            List<int> indexList = new List<int>();
                            var obfuscatedTarget = new ObfuscatedTarget()
                            {
                                Type = type,
                                Method = method
                            };
                            int i = 0;
                            foreach (var instruction in method.Body.Instructions)
                            {
                                if (instruction.Operand != null)
                                {
                                    if (operands.Contains(Convert.ToInt32(instruction.Operand.ToString())))
                                    {
                                        indexList.Add(i);
                                        operands.Remove(Convert.ToInt32(instruction.Operand.ToString()));
                                    }
                                }
                                i++;
                            }
                            if (indexList.Count == operand.Length)
                            {
                                obfuscatedTarget.Indexes = indexList;
                                obfuscatedTargets.Add(obfuscatedTarget);
                            }
                            operands = operand.ToList();
                        }
                    }
                }
                else
                {
                    var nestedTypes = type.NestedTypes;
                    NestedWorker:
                    foreach (var nestedType in nestedTypes)
                    {
                        foreach (var method in type.Methods)
                        {
                            if (method.Body != null)
                            {
                                List<int> indexList = new List<int>();
                                var obfuscatedTarget = new ObfuscatedTarget()
                                {
                                    Type = type,
                                    Method = method
                                };
                                int i = 0;
                                obfuscatedTarget.NestedTypes.Add(nestedType.Name);
                                foreach (var instruction in method.Body.Instructions)
                                {
                                    if (instruction.Operand != null)
                                    {
                                        if (operands.Contains(Convert.ToInt32(instruction.Operand.ToString())))
                                        {
                                            indexList.Add(i);
                                            operands.Remove(Convert.ToInt32(instruction.Operand.ToString()));
                                        }
                                    }
                                    i++;
                                }
                                if (indexList.Count == operand.Length)
                                {
                                    obfuscatedTarget.Indexes = indexList;
                                    obfuscatedTargets.Add(obfuscatedTarget);
                                }
                                operands = operand.ToList();
                            }
                        }
                        if (nestedType.HasNestedTypes)
                        {
                            nestedTypes = nestedType.NestedTypes;
                            goto NestedWorker;
                        }
                    }
                }
            }
            List<Target> targets = new List<Target>();
            foreach (var obfuscatedTarget in obfuscatedTargets)
            {
                Target t = new Target()
                {
                    Namespace = obfuscatedTarget.Type.Namespace,
                    Class = obfuscatedTarget.Type.Name,
                    Method = obfuscatedTarget.Method.Name,
                    NestedClasses = obfuscatedTarget.NestedTypes.ToArray()
                };
                if (obfuscatedTarget.Indexes.Count == 1)
                {
                    t.Index = obfuscatedTarget.Indexes[0];
                }
                else if (obfuscatedTarget.Indexes.Count > 1)
                {
                    t.Indexes = obfuscatedTarget.Indexes.ToArray();
                }

                targets.Add(t);
            }
            return targets.ToArray();
        }

        public static Target[] FindInstructionsByOpcode(OpCode[] opcode)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<string> operands = opcode.Select(o => o.Name).ToList();
            foreach (var type in Module.Types)
            {
                if (!type.HasNestedTypes)
                {
                    foreach (var method in type.Methods)
                    {
                        if (method.Body != null)
                        {
                            List<int> indexList = new List<int>();
                            var obfuscatedTarget = new ObfuscatedTarget()
                            {
                                Type = type,
                                Method = method
                            };
                            int i = 0;
                            foreach (var instruction in method.Body.Instructions)
                            {
                                if (operands.Contains(instruction.OpCode.Name))
                                {
                                    indexList.Add(i);
                                    operands.Remove(instruction.OpCode.Name);
                                }
                                i++;
                            }
                            if (indexList.Count == opcode.Length)
                            {
                                obfuscatedTarget.Indexes = indexList;
                                obfuscatedTargets.Add(obfuscatedTarget);
                            }
                            operands = opcode.Select(o => o.Name).ToList();
                        }
                    }
                }
                else
                {
                    var nestedTypes = type.NestedTypes;
                    NestedWorker:
                    foreach (var nestedType in nestedTypes)
                    {
                        foreach (var method in type.Methods)
                        {
                            if (method.Body != null)
                            {
                                List<int> indexList = new List<int>();
                                var obfuscatedTarget = new ObfuscatedTarget()
                                {
                                    Type = type,
                                    Method = method
                                };
                                int i = 0;
                                obfuscatedTarget.NestedTypes.Add(nestedType.Name);
                                foreach (var instruction in method.Body.Instructions)
                                {
                                    if (operands.Contains(instruction.OpCode.Name))
                                    {
                                        indexList.Add(i);
                                        operands.Remove(instruction.OpCode.Name);
                                    }
                                    i++;
                                }
                                if (indexList.Count == opcode.Length)
                                {
                                    obfuscatedTarget.Indexes = indexList;
                                    obfuscatedTargets.Add(obfuscatedTarget);
                                }
                                operands = opcode.Select(o => o.Name).ToList();
                            }
                        }
                        if (nestedType.HasNestedTypes)
                        {
                            nestedTypes = nestedType.NestedTypes;
                            goto NestedWorker;
                        }
                    }
                }
            }
            List<Target> targets = new List<Target>();
            foreach (var obfuscatedTarget in obfuscatedTargets)
            {
                Target t = new Target()
                {
                    Namespace = obfuscatedTarget.Type.Namespace,
                    Class = obfuscatedTarget.Type.Name,
                    Method = obfuscatedTarget.Method.Name,
                    NestedClasses = obfuscatedTarget.NestedTypes.ToArray()
                };
                if (obfuscatedTarget.Indexes.Count == 1)
                {
                    t.Index = obfuscatedTarget.Indexes[0];
                }
                else if (obfuscatedTarget.Indexes.Count > 1)
                {
                    t.Indexes = obfuscatedTarget.Indexes.ToArray();
                }

                targets.Add(t);
            }
            return targets.ToArray();
        }

        public static Target[] FindInstructionsByOperand(Target target, int[] operand, bool removeIfFound = false)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<int> operands = operand.ToList();
            TypeDef type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef m = null;
            if (target.Method != null)
                m = PatchHelper.FindMethod(type, target.Method);
            if (m != null)
            {
                List<int> indexList = new List<int>();
                var obfuscatedTarget = new ObfuscatedTarget()
                {
                    Type = type,
                    Method = m
                };
                int i = 0;
                foreach (var instruction in m.Body.Instructions)
                {
                    if (instruction.Operand != null)
                    {
                        if (operands.Contains(Convert.ToInt32(instruction.Operand.ToString())))
                        {
                            indexList.Add(i);
                            if (removeIfFound)
                                operands.Remove(Convert.ToInt32(instruction.Operand.ToString()));
                        }
                    }
                    i++;
                }
                if (indexList.Count == operand.Length || removeIfFound == false)
                {
                    obfuscatedTarget.Indexes = indexList;
                    obfuscatedTargets.Add(obfuscatedTarget);
                }
                operands = operand.ToList();
            }
            else
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body != null)
                    {
                        List<int> indexList = new List<int>();
                        var obfuscatedTarget = new ObfuscatedTarget()
                        {
                            Type = type,
                            Method = method
                        };
                        int i = 0;
                        foreach (var instruction in method.Body.Instructions)
                        {
                            if (instruction.Operand != null)
                            {
                                if (operands.Contains(Convert.ToInt32(instruction.Operand.ToString())))
                                {
                                    indexList.Add(i);
                                    if (removeIfFound)
                                        operands.Remove(Convert.ToInt32(instruction.Operand.ToString()));
                                }
                            }
                            i++;
                        }
                        if (indexList.Count == operand.Length || removeIfFound == false)
                        {
                            obfuscatedTarget.Indexes = indexList;
                            obfuscatedTargets.Add(obfuscatedTarget);
                        }
                        operands = operand.ToList();
                    }
                }
            }

            List<Target> targets = new List<Target>();
            foreach (var obfuscatedTarget in obfuscatedTargets)
            {
                Target t = new Target()
                {
                    Namespace = obfuscatedTarget.Type.Namespace,
                    Class = obfuscatedTarget.Type.Name,
                    Method = obfuscatedTarget.Method.Name,
                    NestedClasses = obfuscatedTarget.NestedTypes.ToArray()
                };
                if (obfuscatedTarget.Indexes.Count == 1)
                {
                    t.Index = obfuscatedTarget.Indexes[0];
                }
                else if (obfuscatedTarget.Indexes.Count > 1)
                {
                    t.Indexes = obfuscatedTarget.Indexes.ToArray();
                }

                targets.Add(t);
            }
            return targets.ToArray();
        }

        public static Target[] FindInstructionsByOpcode(Target target, OpCode[] opcode, bool removeIfFound = false)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<string> operands = opcode.Select(o => o.Name).ToList();
            TypeDef type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef m = null;
            if (target.Method != null)
                m = PatchHelper.FindMethod(type, target.Method);
            if (m != null)
            {
                List<int> indexList = new List<int>();
                var obfuscatedTarget = new ObfuscatedTarget()
                {
                    Type = type,
                    Method = m
                };
                int i = 0;
                foreach (var instruction in m.Body.Instructions)
                {
                    if (operands.Contains(instruction.OpCode.Name))
                    {
                        indexList.Add(i);
                        if (removeIfFound)
                            operands.Remove(instruction.OpCode.Name);
                    }
                    i++;
                }
                if (indexList.Count == opcode.Length || removeIfFound == false)
                {
                    obfuscatedTarget.Indexes = indexList;
                    obfuscatedTargets.Add(obfuscatedTarget);
                }
            }
            else
            {
                foreach (var method in type.Methods)
                {
                    if (method.Body != null)
                    {
                        List<int> indexList = new List<int>();
                        var obfuscatedTarget = new ObfuscatedTarget()
                        {
                            Type = type,
                            Method = method
                        };
                        int i = 0;
                        foreach (var instruction in method.Body.Instructions)
                        {
                            if (operands.Contains(instruction.OpCode.Name))
                            {
                                indexList.Add(i);
                                if (removeIfFound)
                                    operands.Remove(instruction.OpCode.Name);
                            }
                            i++;
                        }
                        if (indexList.Count == opcode.Length || removeIfFound == false)
                        {
                            obfuscatedTarget.Indexes = indexList;
                            obfuscatedTargets.Add(obfuscatedTarget);
                        }
                        operands = opcode.Select(o => o.Name).ToList();
                    }
                }
            }

            List<Target> targets = new List<Target>();
            foreach (var obfuscatedTarget in obfuscatedTargets)
            {
                Target t = new Target()
                {
                    Namespace = obfuscatedTarget.Type.Namespace,
                    Class = obfuscatedTarget.Type.Name,
                    Method = obfuscatedTarget.Method.Name,
                    NestedClasses = obfuscatedTarget.NestedTypes.ToArray()
                };
                if (obfuscatedTarget.Indexes.Count == 1)
                {
                    t.Index = obfuscatedTarget.Indexes[0];
                }
                else if (obfuscatedTarget.Indexes.Count > 1)
                {
                    t.Indexes = obfuscatedTarget.Indexes.ToArray();
                }

                targets.Add(t);
            }
            return targets.ToArray();
        }

        public static MemberRef BuildMemberRef(string ns, string cs, string name, Patcher.MemberRefType type)
        {
            TypeRef consoleRef = new TypeRefUser(Module, ns, cs, Module.CorLibTypes.AssemblyRef);
            if (type == Patcher.MemberRefType.Static)
            {
                return new MemberRefUser(Module, name,
                    MethodSig.CreateStatic(Module.CorLibTypes.Void, Module.CorLibTypes.String),
                    consoleRef);
            }
            else
            {
                return new MemberRefUser(Module, name,
                   MethodSig.CreateInstance(Module.CorLibTypes.Void, Module.CorLibTypes.String),
                   consoleRef);
            }
        }

        public static void ReplaceInstruction(Target target)
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
            var type = FindType(target.Namespace + "." + target.Class, nestedClasses);
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

        public static void RemoveInstruction(Target target)
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
            var type = FindType(target.Namespace + "." + target.Class, nestedClasses);
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

        public static Instruction[] GetInstructions(Target target)
        {
            var type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = PatchHelper.FindMethod(type, target.Method);
            return (Instruction[])method.Body.Instructions;
        }

        public static void PatchOperand(Target target, string operand)
        {
            TypeDef type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = PatchHelper.FindMethod(type, target.Method);
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

        public static void PatchOperand(Target target, int operand)
        {
            TypeDef type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = PatchHelper.FindMethod(type, target.Method);
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

        public static void PatchOperand(Target target, string[] operand)
        {
            TypeDef type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = PatchHelper.FindMethod(type, target.Method);
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

        public static void PatchOperand(Target target, int[] operand)
        {
            TypeDef type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = PatchHelper.FindMethod(type, target.Method);
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

        public static string GetOperand(Target target)
        {
            TypeDef type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = PatchHelper.FindMethod(type, target.Method);
            return method.Body.Instructions[target.Index].Operand.ToString();
        }

        public static int FindInstruction(Target target, Instruction instruction, int occurence)
        {
            occurence--; // Fix the occurence, e.g. second occurence must be 1 but hoomans like to write like they speak so why don't assist them?
            var type = PatchHelper.FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = PatchHelper.FindMethod(type, target.Method);
            var instructions = method.Body.Instructions;
            int index = 0;
            int occurenceCounter = 0;
            foreach (var i in instructions)
            {
                if (i.Operand == null && instruction.Operand == null)
                {
                    if (i.OpCode.Name == instruction.OpCode.Name && occurenceCounter < occurence)
                    {
                        occurenceCounter++;
                    }
                    else if (i.OpCode.Name == instruction.OpCode.Name && occurenceCounter == occurence)
                    {
                        return index;
                    }
                }
                else if (i.OpCode.Name == instruction.OpCode.Name && i.Operand.ToString() == instruction.Operand.ToString() &&
                         occurenceCounter < occurence)
                {
                    occurenceCounter++;
                }
                else if (i.OpCode.Name == instruction.OpCode.Name && i.Operand.ToString() == instruction.Operand.ToString() &&
                         occurenceCounter == occurence)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    }
}
