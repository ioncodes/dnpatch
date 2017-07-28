using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnpatch.Enums;
using dnpatch.Misc;
using dnpatch.Types;

namespace dnpatch.Processors
{
    public class ILProcessor
    {
        protected Assembly _assembly;

        internal ILProcessor(Assembly assembly)
		{
            _assembly = assembly;
		}

        #region Write

        public void Clear()
        {
            if (_assembly.AssemblyModel.Method != null)
            {
                _assembly.AssemblyModel.Method.Body.Instructions.Clear();
            }
            else
            {
                if (_assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
                {
                    _assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Clear();
                }
                else
                {
                    _assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Clear();
                }
            }
        }

        public void Append(InstructionSet instructionSet)
        {
            if(instructionSet.Indices != null)
            {
                Append(instructionSet.Instructions, instructionSet.Indices);
            }
            else
            {
                Append(instructionSet.Instructions);
            }
        }


        public void Append(Instruction[] instructions, int[] indices)
        {
            if (_assembly.AssemblyModel.Method != null)
            {
                for (int i = 0; i < instructions.Length; i++)
                {
                    var instruction = instructions[i];
                    var index = indices[i];
                    _assembly.AssemblyModel.Method.Body.Instructions.Insert(index, instruction);
                }
            }
            else
            {
                if (_assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
                {
                    for (int i = 0; i < instructions.Length; i++)
                    {
                        var instruction = instructions[i];
                        var index = indices[i];
                        _assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Insert(index, instruction);
                    }
                }
                else
                {
                    for (int i = 0; i < instructions.Length; i++)
                    {
                        var instruction = instructions[i];
                        var index = indices[i];
                        _assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Insert(index, instruction);
                    }
                }
            }
        }

        public void Append(Instruction[] instructions)
        {
			if (_assembly.AssemblyModel.Method != null)
			{
                foreach(var instruction in instructions)
                {
                    _assembly.AssemblyModel.Method.Body.Instructions.Add(instruction);
                }
            }
			else
			{
				if (_assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
				{
					foreach (var instruction in instructions)
					{
                        _assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Add(instruction);
					}
				}
				else
				{
					foreach (var instruction in instructions)
					{
						_assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Add(instruction);
					}
				}
			}
        }

        public void Overwrite(Instruction[] instructions)
        {
            Clear();
            Append(instructions);
        }

		public void Overwrite(Instruction[] instructions, int[] indices)
		{
            for (int i = 0; i < instructions.Length; i++)
			{
				var instruction = instructions[i];
                var index = indices[i];
				Write(instruction, index);
			}
		}

        public void Overwrite(InstructionSet instructionSet)
		{
            if (instructionSet.Indices != null)
            {
                Overwrite(instructionSet.Instructions, instructionSet.Indices);
            }
            else
            {
                Append(instructionSet.Instructions);
            }
		}

        public void Write(Instruction instruction, int index)
        {
			if (_assembly.AssemblyModel.Method != null)
			{
                _assembly.AssemblyModel.Method.Body.Instructions[index] = instruction;
			}
			else
			{
				if (_assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
				{
					_assembly.AssemblyModel.Property.GetMethod.Body.Instructions[index] = instruction;
				}
				else
				{
					_assembly.AssemblyModel.Property.SetMethod.Body.Instructions[index] = instruction;
				}
			}
        }

        #endregion

        #region Read

        public List<MethodDef> FindMethod(Instruction[] instructions, SearchMode searchMode)
        {
            List<MethodDef> methods = new List<MethodDef>();
            foreach (var method in GetAllMethods())
            {
                if(!method.HasBody) continue;
                if (searchMode == SearchMode.Consecutive && method.Body.Instructions.ContainsSequence(instructions))
                    methods.Add(method);
                else if(searchMode == SearchMode.Default && !method.Body.Instructions.Except(instructions).Any())
                    methods.Add(method);
            }
            return methods;
        }

        public List<MethodDef> FindMethod(OpCode[] opcodes, SearchMode searchMode)
        {
            List<MethodDef> methods = new List<MethodDef>();
            foreach (var method in GetAllMethods())
            {
                if (!method.HasBody) continue;
                if (searchMode == SearchMode.Consecutive && method.Body.Instructions.Select(ins => ins.OpCode).ToList().ContainsSequence(opcodes))
                    methods.Add(method);
                else if (searchMode == SearchMode.Default && !method.Body.Instructions.Select(ins => ins.OpCode).ToList().Any())
                    methods.Add(method);
            }
            return methods;
        }

        private List<MethodDef> GetAllMethods()
        {
            List<MethodDef> methods = new List<MethodDef>();
            foreach (var type in _assembly.AssemblyData.Module.Types)
            {
                methods.AddRange(type.Methods.Where(i => i.HasBody));
            }
            return methods;
        }

        #endregion
    }
}
