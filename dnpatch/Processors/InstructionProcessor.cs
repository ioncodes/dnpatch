using System.Reflection;
using dnlib.DotNet;

namespace dnpatch.Processors
{
    public class InstructionProcessor
    {
		protected Assembly _assembly;

        internal InstructionProcessor(Assembly assembly)
		{
			_assembly = assembly;
		}

        public IMethod BuildCall(MethodDef method)
		{
			IMethod meth = _assembly.AssemblyData.Importer.Import(method);
			return meth;
		}

		public IMethod BuildCall(MethodInfo method)
		{
			IMethod meth = _assembly.AssemblyData.Importer.Import(method);
			return meth;
		}
    }
}
