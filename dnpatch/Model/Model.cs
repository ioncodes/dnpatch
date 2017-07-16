using System;
using System.Linq;
using System.Collections.Generic;
using dnlib.DotNet;

namespace dnpatch
{
    public class Model
    {
        protected Assembly _assembly;

        internal Model(Assembly assembly)
        {
            _assembly = assembly;
        }

		public void SetNamespace(string @namespace)
		{
			_assembly.AssemblyModel.Namespace = @namespace;
		}

		public void SetType(string classPath)
		{
			string path = $"{_assembly.AssemblyModel.Namespace}.{classPath}";
			TypeDef type = _assembly.AssemblyData.Module.FindReflection(path);
			_assembly.AssemblyModel.Type = type ?? throw new Exception($"Type '{path}' does not exist.");
		}

		public void SetField(string fieldName)
		{
			_assembly.AssemblyModel.Field =_assembly.AssemblyModel.Type.FindField(fieldName) ?? throw new Exception($"Field '{_assembly.AssemblyModel.Type.FullName}.{fieldName}' does not exist.");
			VerifyModel();
		}

		public void SetMethod(string methodName)
		{
			_assembly.AssemblyModel.Method = _assembly.AssemblyModel.Type.FindMethod(methodName) ?? throw new Exception($"Method '{_assembly.AssemblyModel.Type.FullName}.{methodName}' does not exist.");
			VerifyModel();
		}

		public void SetProperty(string propertyName, PropertyMethod propertyMethod)
		{
			_assembly.AssemblyModel.Property = _assembly.AssemblyModel.Type.FindProperty(propertyName) ?? throw new Exception($"Property '{_assembly.AssemblyModel.Type.FullName}.{propertyName}' does not exist.");
			_assembly.AssemblyModel.PropertyMethod = propertyMethod;
			VerifyModel();
		}

		public void SetEvent(string eventName)
		{
			_assembly.AssemblyModel.Event = _assembly.AssemblyModel.Type.FindEvent(eventName) ?? throw new Exception($"Event '{_assembly.AssemblyModel.Type.FullName}.{eventName}' does not exist.");
			VerifyModel();
		}

		private void VerifyModel()
		{
			if (new List<bool>()
			{
				_assembly.AssemblyModel.Event != null,
				_assembly.AssemblyModel.Field != null,
				_assembly.AssemblyModel.Method != null,
				_assembly.AssemblyModel.Property != null
			}.Count(b => b) > 1) throw new Exception($"Check your AssemblyModel in Assembly '{_assembly.AssemblyInfo.InternalName}'. Multiple assignments found for properties: Event, Field, Method, Property");
		}
    }
}
