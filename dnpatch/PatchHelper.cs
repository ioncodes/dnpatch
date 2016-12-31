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
    internal class PatchHelper
    {
        private readonly ModuleDef _module;
        private readonly string _file;
        private readonly bool _keepOldMaxStack = false;

        public PatchHelper(string file)
        {
            _file = file;
            _module = ModuleDefMD.Load(file);
        }

        public PatchHelper(string file, bool keepOldMaxStack)
        {
            _file = file;
            _module = ModuleDefMD.Load(file);
            _keepOldMaxStack = keepOldMaxStack;
        }

        public  void PatchAndClear(Target target)
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

        public  void PatchOffsets(Target target)
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

        public  TypeDef FindType(string classPath, string[] nestedClasses)
        {
            if (classPath.First() == '.')
                classPath = classPath.Remove(0, 1);
            foreach (var module in _module.Assembly.Modules)
            {
                foreach (var type in _module.Types)
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

        public  MethodDef FindMethod(TypeDef type, string methodName)
        {
            return type.Methods.FirstOrDefault(m => methodName == m.Name);
        }

        public  Target FixTarget(Target target)
        {
            target.Indexes = new int[] { };
            target.Index = -1;
            target.Instruction = null;
            return target;
        }

        public  void Save(string name)
        {
            if (_keepOldMaxStack)
                _module.Write(name, new ModuleWriterOptions(_module)
                {
                    MetaDataOptions = {Flags = MetaDataFlags.KeepOldMaxStack}
                });
            else
                _module.Write(name);
        }

        public  void Save(bool backup)
        {
            if (_keepOldMaxStack)
                _module.Write(_file + ".tmp", new ModuleWriterOptions(_module)
                {
                    MetaDataOptions = { Flags = MetaDataFlags.KeepOldMaxStack }
                });
            else
                _module.Write(_file + ".tmp");
            _module.Dispose();
            if (backup)
            {
                if (File.Exists(_file + ".bak"))
                {
                    File.Delete(_file + ".bak");
                }
                File.Move(_file, _file + ".bak");
            }
            else
            {
                File.Delete(_file);
            }
            File.Move(_file + ".tmp", _file);
        }

        public Target[] FindInstructionsByOperand(string[] operand)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<string> operands = operand.ToList();
            foreach (var type in _module.Types)
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

        public  Target[] FindInstructionsByOperand(int[] operand)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<int> operands = operand.ToList();
            foreach (var type in _module.Types)
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

        public  Target[] FindInstructionsByOpcode(OpCode[] opcode)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<string> operands = opcode.Select(o => o.Name).ToList();
            foreach (var type in _module.Types)
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

        public  Target[] FindInstructionsByOperand(Target target, int[] operand, bool removeIfFound = false)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<int> operands = operand.ToList();
            TypeDef type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef m = null;
            if (target.Method != null)
                m = FindMethod(type, target.Method);
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

        public  Target[] FindInstructionsByOpcode(Target target, OpCode[] opcode, bool removeIfFound = false)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<string> operands = opcode.Select(o => o.Name).ToList();
            TypeDef type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef m = null;
            if (target.Method != null)
                m = FindMethod(type, target.Method);
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

        public  MemberRef BuildMemberRef(string ns, string cs, string name, Patcher.MemberRefType type)
        {
            TypeRef consoleRef = new TypeRefUser(_module, ns, cs, _module.CorLibTypes.AssemblyRef);
            if (type == Patcher.MemberRefType.Static)
            {
                return new MemberRefUser(_module, name,
                    MethodSig.CreateStatic(_module.CorLibTypes.Void, _module.CorLibTypes.String),
                    consoleRef);
            }
            else
            {
                return new MemberRefUser(_module, name,
                   MethodSig.CreateInstance(_module.CorLibTypes.Void, _module.CorLibTypes.String),
                   consoleRef);
            }
        }

        public  void ReplaceInstruction(Target target)
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

        public  void RemoveInstruction(Target target)
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

        public  Instruction[] GetInstructions(Target target)
        {
            var type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            return (Instruction[])method.Body.Instructions;
        }

        public  void PatchOperand(Target target, string operand)
        {
            TypeDef type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
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

        public  void PatchOperand(Target target, int operand)
        {
            TypeDef type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
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

        public  void PatchOperand(Target target, string[] operand)
        {
            TypeDef type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
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

        public  void PatchOperand(Target target, int[] operand)
        {
            TypeDef type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
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

        public  string GetOperand(Target target)
        {
            TypeDef type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
            return method.Body.Instructions[target.Index].Operand.ToString();
        }

        public  int FindInstruction(Target target, Instruction instruction, int occurence)
        {
            occurence--; // Fix the occurence, e.g. second occurence must be 1 but hoomans like to write like they speak so why don't assist them?
            var type = FindType(target.Namespace + "." + target.Class, target.NestedClasses);
            MethodDef method = FindMethod(type, target.Method);
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
