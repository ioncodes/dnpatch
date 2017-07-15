using dnlib.DotNet.Emit;

namespace dnpatch
{
    public class ILProcessor
    {
        public void Clear(Assembly assembly)
        {
            if (assembly.AssemblyModel.Method != null)
            {
                assembly.AssemblyModel.Method.Body.Instructions.Clear();
            }
            else
            {
                if (assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
                {
                    assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Clear();
                }
                else
                {
                    assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Clear();
                }
            }
        }

        public void Append(Assembly assembly, InstructionSet instructionSet)
        {
            if(instructionSet.Indices != null)
            {
                Append(assembly, instructionSet.Instructions, instructionSet.Indices);
            }
            else
            {
                Append(assembly, instructionSet.Instructions);
            }
        }


        public void Append(Assembly assembly, Instruction[] instructions, int[] indices)
        {
            if (assembly.AssemblyModel.Method != null)
            {
                for (int i = 0; i < instructions.Length; i++)
                {
                    var instruction = instructions[i];
                    var index = indices[i];
                    assembly.AssemblyModel.Method.Body.Instructions.Insert(index, instruction);
                }
            }
            else
            {
                if (assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
                {
                    for (int i = 0; i < instructions.Length; i++)
                    {
                        var instruction = instructions[i];
                        var index = indices[i];
                        assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Insert(index, instruction);
                    }
                }
                else
                {
                    for (int i = 0; i < instructions.Length; i++)
                    {
                        var instruction = instructions[i];
                        var index = indices[i];
                        assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Insert(index, instruction);
                    }
                }
            }
        }

        public void Append(Assembly assembly, Instruction[] instructions)
        {
			if (assembly.AssemblyModel.Method != null)
			{
                foreach(var instruction in instructions)
                {
                    assembly.AssemblyModel.Method.Body.Instructions.Add(instruction);
                }
            }
			else
			{
				if (assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
				{
					foreach (var instruction in instructions)
					{
                        assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Add(instruction);
					}
				}
				else
				{
					foreach (var instruction in instructions)
					{
						assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Add(instruction);
					}
				}
			}
        }

        public void Overwrite(Assembly assembly, Instruction[] instructions)
        {
            Clear(assembly);
            Append(assembly, instructions);
        }

		public void Overwrite(Assembly assembly, Instruction[] instructions, int[] indices)
		{
            for (int i = 0; i < instructions.Length; i++)
			{
				var instruction = instructions[i];
                var index = indices[i];
				Write(assembly, instruction, index);
			}
		}

        public void Overwrite(Assembly assembly, InstructionSet instructionSet)
		{
            if (instructionSet.Indices != null)
            {
                Overwrite(assembly, instructionSet.Instructions, instructionSet.Indices);
            }
            else
            {
                Append(assembly, instructionSet.Instructions);
            }
		}

        public void Write(Assembly assembly, Instruction instruction, int index)
        {
			if (assembly.AssemblyModel.Method != null)
			{
                assembly.AssemblyModel.Method.Body.Instructions[index] = instruction;
			}
			else
			{
				if (assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
				{
					assembly.AssemblyModel.Property.GetMethod.Body.Instructions[index] = instruction;
				}
				else
				{
					assembly.AssemblyModel.Property.SetMethod.Body.Instructions[index] = instruction;
				}
			}
        }
    }
}
