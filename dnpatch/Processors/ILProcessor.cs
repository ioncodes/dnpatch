using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnpatch.Enums;
using dnpatch.Misc;
using dnpatch.Types;

namespace dnpatch.Processors
{
    /// <summary>
    /// IL processing engine
    /// </summary>
    public class ILProcessor
    {
        /// <summary>
        /// The assembly
        /// </summary>
        protected Assembly Assembly;

        internal ILProcessor(Assembly assembly)
		{
            Assembly = assembly;
		}

        #region Write

        /// <summary>
        /// Clears the method body
        /// </summary>
        public void Clear()
        {
            if (Assembly.AssemblyModel.Method != null)
            {
                Assembly.AssemblyModel.Method.Body.Instructions.Clear();
            }
            else
            {
                if (Assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
                {
                    Assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Clear();
                }
                else
                {
                    Assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Clear();
                }
            }
        }

        /// <summary>
        /// Appends the specified instruction set.
        /// </summary>
        /// <param name="instructionSet">The instruction set.</param>
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

        /// <summary>
        /// Appends the specified instructions.
        /// </summary>
        /// <param name="instructions">The instructions.</param>
        /// <param name="indices">The indices.</param>
        public void Append(Instruction[] instructions, int[] indices)
        {
            if (Assembly.AssemblyModel.Method != null)
            {
                for (int i = 0; i < instructions.Length; i++)
                {
                    var instruction = instructions[i];
                    var index = indices[i];
                    Assembly.AssemblyModel.Method.Body.Instructions.Insert(index, instruction);
                }
            }
            else
            {
                if (Assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
                {
                    for (int i = 0; i < instructions.Length; i++)
                    {
                        var instruction = instructions[i];
                        var index = indices[i];
                        Assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Insert(index, instruction);
                    }
                }
                else
                {
                    for (int i = 0; i < instructions.Length; i++)
                    {
                        var instruction = instructions[i];
                        var index = indices[i];
                        Assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Insert(index, instruction);
                    }
                }
            }
        }

        /// <summary>
        /// Appends the specified instructions.
        /// </summary>
        /// <param name="instructions">The instructions.</param>
        public void Append(Instruction[] instructions)
        {
			if (Assembly.AssemblyModel.Method != null)
			{
                foreach(var instruction in instructions)
                {
                    Assembly.AssemblyModel.Method.Body.Instructions.Add(instruction);
                }
            }
			else
			{
				if (Assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
				{
					foreach (var instruction in instructions)
					{
                        Assembly.AssemblyModel.Property.GetMethod.Body.Instructions.Add(instruction);
					}
				}
				else
				{
					foreach (var instruction in instructions)
					{
						Assembly.AssemblyModel.Property.SetMethod.Body.Instructions.Add(instruction);
					}
				}
			}
        }

        /// <summary>
        /// Overwrites the body with the specified instructions.
        /// </summary>
        /// <param name="instructions">The instructions.</param>
        public void Overwrite(Instruction[] instructions)
        {
            Clear();
            Append(instructions);
        }

        /// <summary>
        /// Overwrites the body with the specified instructions.
        /// </summary>
        /// <param name="instructions">The instructions.</param>
        /// <param name="indices">The indices.</param>
        public void Overwrite(Instruction[] instructions, int[] indices)
		{
            for (int i = 0; i < instructions.Length; i++)
			{
				var instruction = instructions[i];
                var index = indices[i];
				Write(instruction, index);
			}
		}

        /// <summary>
        /// Overwrites the body with the specified instructions.
        /// </summary>
        /// <param name="instructionSet">The instruction set.</param>
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

        /// <summary>
        /// Writes the specified instruction at the specified index.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <param name="index">The index.</param>
        public void Write(Instruction instruction, int index)
        {
			if (Assembly.AssemblyModel.Method != null)
			{
                Assembly.AssemblyModel.Method.Body.Instructions[index] = instruction;
			}
			else
			{
				if (Assembly.AssemblyModel.PropertyMethod == PropertyMethod.Get)
				{
					Assembly.AssemblyModel.Property.GetMethod.Body.Instructions[index] = instruction;
				}
				else
				{
					Assembly.AssemblyModel.Property.SetMethod.Body.Instructions[index] = instruction;
				}
			}
        }

        #endregion

        #region Read

        /// <summary>
        /// Finds the method.
        /// </summary>
        /// <param name="instructions">The instructions.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <returns>A list of the found methods</returns>
        public List<MethodDef> FindMethod(Instruction[] instructions, SearchMode searchMode) // TODO: Make it faster and more reliable
        {
            List<MethodDef> assemblyMethods = Assembly.AssemblyInfo.PreloadData
                ? Assembly.AssemblyData.Methods
                : Assembly.GetAllMethods();
            return assemblyMethods.Where(method => method.HasBody).Where(Find).ToList();

            bool Find(MethodDef method)
            {
                if (searchMode == SearchMode.Consecutive)
                {
                    bool hasOpCodes = method.Body.Instructions.Select(o => o.OpCode).ToList()
                        .ContainsSequence(instructions.Select(o => o.OpCode).ToList());
                    bool hasOperand = method.Body.Instructions.Select(o => o.Operand).Where(o => o != null).ToList()
                        .ContainsSequence(instructions.Select(o => o.Operand).Where(o => o != null).ToList());
                    return hasOperand && hasOpCodes;
                }
                else
                {
                    bool hasOpCodes = !instructions.Select(o => o.OpCode).ToList().Except(method.Body.Instructions.Select(o => o.OpCode).ToList()).Any();
                    bool hasOperand = !instructions.Select(o => o.Operand).Where(o => o != null).ToList().Except(method.Body.Instructions.Select(o => o.Operand).Where(o => o != null).ToList()).Any();
                    return hasOperand && hasOpCodes;
                }
            }
        }

        /// <summary>
        /// Finds the method.
        /// </summary>
        /// <param name="opcodes">The opcodes.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <returns>A list of the found methods</returns>
        public List<MethodDef> FindMethod(OpCode[] opcodes, SearchMode searchMode)
        {
            List<MethodDef> methods = new List<MethodDef>();
            List<MethodDef> assemblyMethods = Assembly.AssemblyInfo.PreloadData
                ? Assembly.AssemblyData.Methods
                : Assembly.GetAllMethods();
            foreach (var method in assemblyMethods)
            {
                if (!method.HasBody) continue;
                if (searchMode == SearchMode.Consecutive && method.Body.Instructions.Select(ins => ins.OpCode).ToList().ContainsSequence(opcodes))
                    methods.Add(method);
                else if (searchMode == SearchMode.Default && !opcodes.ToList().Except(method.Body.Instructions.Select(ins => ins.OpCode).ToList()).Any())
                    methods.Add(method);
            }
            return methods;
        }

        #endregion
    }
}
