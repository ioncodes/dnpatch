using System.Reflection;
using dnlib.DotNet;
using Assembly = dnpatch.Types.Assembly;

namespace dnpatch.Processors
{
    /// <summary>
    /// Instruction processing engine
    /// </summary>
    public class InstructionProcessor
    {
        /// <summary>
        /// The assembly
        /// </summary>
        protected Assembly Assembly;

        internal InstructionProcessor(Assembly assembly)
		{
			Assembly = assembly;
		}

        /// <summary>
        /// Imports the method into the assembly and builds a call.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The call</returns>
        public IMethod BuildCall(MethodDef method)
		{
			IMethod meth = Assembly.AssemblyData.Importer.Import(method);
			return meth;
		}

        /// <summary>
        /// Imports the method into the assembly and builds a call.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The call</returns>
        public IMethod BuildCall(MethodInfo method)
		{
			IMethod meth = Assembly.AssemblyData.Importer.Import(method);
			return meth;
		}
    }
}
