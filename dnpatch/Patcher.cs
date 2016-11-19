using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void PatchAndClear(Target target)
        {
            var types = FindType(module.Assembly, target.Namespace + "." + target.Class);
            TypeDef t = null;
            if (target.NestedClasses != null)
            {
                foreach (var nc in target.NestedClasses)
                {
                    foreach (var type in types.NestedTypes)
                    { 
                        if (type.Name == nc)
                        {
                            t = type;
                        }
                    }
                }
            }
            else
            {
                t = types;
            }

            // t is now the target class

            foreach (var m in t.Methods)
            {
                if (target.Method == m.Name)
                {
                    if (target.Instructions != null)
                    {
                        var instructions = m.Body.Instructions;
                        instructions.Clear();
                        for (int i = 0; i < target.Instructions.Length; i++)
                        {
                            instructions.Insert(i, target.Instructions[i]);
                        }
                    }
                    else
                    {
                        throw new Exception("No instructions specified");
                    }
                }
            }
        }

        private void PatchOffsets(Target target)
        {
            var types = FindType(module.Assembly, target.Namespace + "." + target.Class);
            TypeDef t = null;
            if (target.NestedClasses != null)
            {
                foreach (var nc in target.NestedClasses)
                {
                    if (t == null)
                    {
                        if (!types.HasNestedTypes) continue;
                        Console.WriteLine(types.NestedTypes[0]);
                        foreach (var type in types.NestedTypes)
                        {
                            if (type.Name == nc)
                            {
                                t = type;
                                Console.WriteLine(t.Name);
                            }
                        }
                    }
                    else
                    {
                        if (!t.HasNestedTypes) continue;
                        Console.WriteLine(t.NestedTypes[0]);
                        foreach (var type in t.NestedTypes)
                        {
                            if (type.Name == nc)
                            {
                                t = type;
                                Console.WriteLine(t.Name);
                            }
                        }
                    }
                }
            }
            else
            {
                t = types;
            }

            // t is now the target class

            foreach (var m in t.Methods)
            {
                if (target.Method == m.Name)
                {
                    var instructions = m.Body.Instructions;
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
            }
        }

        public void Save(string name)
        {
            //if (Path.GetFileName(file) == Path.GetFileName(name))
            //{
            //    throw new Exception("Save name cannot be the same as the assemblies name, use the overloaded function for this");
            //}
            module.Write(name);
        }

        public void Save(bool backup)
        {
            module.Write(file+".tmp");
            module.Dispose();
            if (backup)
            {
                if (File.Exists(file + ".bak"))
                {
                    File.Delete(file + ".bak");
                }
                File.Move(file, file+".bak");
            }
            else
            {
                File.Delete(file);
            }
            File.Move(file + ".tmp", file);
        }

        public Instruction FindInstruction(Instruction[] instructions, Code code)
        {
            return instructions.Single(i => i.OpCode.Code == code);
        }

        public MemberRef BuildMemberRef(string ns, string cs, string name) // debug stuff
        {
            TypeRef consoleRef = new TypeRefUser(module, ns, cs, module.CorLibTypes.AssemblyRef);
            return new MemberRefUser(module, name,
                        MethodSig.CreateStatic(module.CorLibTypes.Void, module.CorLibTypes.String),
                        consoleRef);
        }

        private TypeDef FindType(AssemblyDef asm, string classPath)
        {
            foreach (var module in asm.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName == classPath)
                        return type;
                }
            }
            return null;
        }
    }
}
