using dnlib.DotNet.Emit;
using dnpatch.Enums;
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
    }
}
