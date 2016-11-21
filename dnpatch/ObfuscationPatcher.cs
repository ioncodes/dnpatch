using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace dnpatch
{
    public class ObfuscationPatcher
    {
        private string file;
        private readonly ModuleDefMD module;
        private ModuleWriterOptions mwo;

        public ObfuscationPatcher(string file, bool keepOldMaxStack)
        {
            this.file = file;
            module = ModuleDefMD.Load(file);
            if (keepOldMaxStack)
            {
                mwo = new ModuleWriterOptions(module)
                {
                    MetaDataOptions = {Flags = MetaDataFlags.KeepOldMaxStack}
                };
            }
        }

        public void Patch(Target target)
        {
            if ((target.Indexes != null || target.Index != -1) && (target.Instruction != null || target.Instructions != null))
            {
                PatchOffsets(target);
            }
            else if ((target.Index == -1 && target.Indexes == null) && (target.Instruction != null || target.Instructions != null))
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
            module.Write(name, mwo);
        }

        public void Save(bool backup)
        {
            module.Write(file + ".tmp", mwo);
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

        public Target[] FindInstructionsByOperand(string[] operand)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<string> operands = operand.ToList();
            foreach (var type in module.Types)
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
                targets.Add(new Target()
                {
                    Namespace = obfuscatedTarget.Type.Namespace,
                    Class = obfuscatedTarget.Type.Name,
                    Method = obfuscatedTarget.Method.Name,
                    Indexes = obfuscatedTarget.Indexes.ToArray(),
                    NestedClasses = obfuscatedTarget.NestedTypes.ToArray()
                });
            }
            return targets.ToArray();
        }

        public Target[] FindInstructionsByOperand(int[] operand)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<int> operands = operand.ToList();
            foreach (var type in module.Types)
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
                targets.Add(new Target()
                {
                    Namespace = obfuscatedTarget.Type.Namespace,
                    Class = obfuscatedTarget.Type.Name,
                    Method = obfuscatedTarget.Method.Name,
                    Indexes = obfuscatedTarget.Indexes.ToArray(),
                    NestedClasses = obfuscatedTarget.NestedTypes.ToArray()
                });
            }
            return targets.ToArray();
        }

        public Target[] FindInstructionsByOpcode(OpCode[] opcode)
        {
            List<ObfuscatedTarget> obfuscatedTargets = new List<ObfuscatedTarget>();
            List<string> operands = opcode.Select(o => o.Name).ToList();
            foreach (var type in module.Types)
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
                                    if (operands.Contains(instruction.OpCode.Name))
                                    {
                                        indexList.Add(i);
                                        operands.Remove(instruction.OpCode.Name);
                                    }
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
                                    if (instruction.Operand != null)
                                    {
                                        if (operands.Contains(instruction.OpCode.Name))
                                        {
                                            indexList.Add(i);
                                            operands.Remove(instruction.OpCode.Name);
                                        }
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
                targets.Add(new Target()
                {
                    Namespace = obfuscatedTarget.Type.Namespace,
                    Class = obfuscatedTarget.Type.Name,
                    Method = obfuscatedTarget.Method.Name,
                    Indexes = obfuscatedTarget.Indexes.ToArray(),
                    NestedClasses = obfuscatedTarget.NestedTypes.ToArray()
                });
            }
            return targets.ToArray();
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
                nestedClasses = new[] { target.NestedClass };
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
    }
}
