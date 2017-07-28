using System.Reflection;
using dnlib.DotNet;
using Assembly = dnpatch.Types.Assembly;

namespace dnpatch.Processors
{
    public class InstructionProcessor
    {
		protected Assembly _assembly;

        internal InstructionProcessor(Assembly assembly)
		{
			_assembly = assembly;
		}

        /// <summary>
        /// Imports the method into the assembly and builds a call.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The call</returns>
        public IMethod BuildCall(MethodDef method)
		{
			IMethod meth = _assembly.AssemblyData.Importer.Import(method);
			return meth;
		}

        /// <summary>
        /// Imports the method into the assembly and builds a call.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The call</returns>
        public IMethod BuildCall(MethodInfo method)
		{
			IMethod meth = _assembly.AssemblyData.Importer.Import(method);
			return meth;
		}
    }
}
