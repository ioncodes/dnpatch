using System;
using System.Reflection;
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

        public void SetNamespace(string @namespace) // TODO: Remove this, I never needed it...
		{
			_assembly.AssemblyModel.Namespace = @namespace;
		}

		public void SetType(string classPath)
		{
			string path = $"{_assembly.AssemblyModel.Namespace}.{classPath}";
			TypeDef type = _assembly.AssemblyData.Module.FindReflection(path);
			_assembly.AssemblyModel.Type = type ?? throw new Exception($"Type '{path}' does not exist.");
		}

		public void SetType(Type type)
		{
            _assembly.AssemblyModel.Type = FindType(type);
		}

        public void SetType(TypeDef type)
		{
            _assembly.AssemblyModel.Type = type;
		}

		public void SetField(string fieldName)
		{
			_assembly.AssemblyModel.Field = _assembly.AssemblyModel.Type.FindField(fieldName) ?? throw new Exception($"Field '{_assembly.AssemblyModel.Type.FullName}.{fieldName}' does not exist.");
			VerifyModel();
		}

        public void SetField(FieldInfo field)
		{
            _assembly.AssemblyModel.Field = FindField(field);
			VerifyModel();
		}

        public void SetField(FieldDef field)
		{
            _assembly.AssemblyModel.Field = field;
			VerifyModel();
		}

		public void SetMethod(string methodName)
		{
			_assembly.AssemblyModel.Method = _assembly.AssemblyModel.Type.FindMethod(methodName) ?? throw new Exception($"Method '{_assembly.AssemblyModel.Type.FullName}.{methodName}' does not exist.");
			VerifyModel();
		}

        public void SetMethod(MethodInfo method)
		{
            _assembly.AssemblyModel.Method = FindMethod(method);
			VerifyModel();
		}

        public void SetMethod(MethodDef method)
		{
            _assembly.AssemblyModel.Method = method;
			VerifyModel();
		}

		public void SetProperty(string propertyName, PropertyMethod propertyMethod)
		{
			_assembly.AssemblyModel.Property = _assembly.AssemblyModel.Type.FindProperty(propertyName) ?? throw new Exception($"Property '{_assembly.AssemblyModel.Type.FullName}.{propertyName}' does not exist.");
			_assembly.AssemblyModel.PropertyMethod = propertyMethod;
			VerifyModel();
		}

        public void SetProperty(PropertyInfo property)
		{
            _assembly.AssemblyModel.Property = FindProperty(property);
			VerifyModel();
		}

        public void SetProperty(PropertyDef property)
		{
            _assembly.AssemblyModel.Property = property;
			VerifyModel();
		}

		public void SetEvent(string eventName)
		{
			_assembly.AssemblyModel.Event = _assembly.AssemblyModel.Type.FindEvent(eventName) ?? throw new Exception($"Event '{_assembly.AssemblyModel.Type.FullName}.{eventName}' does not exist.");
			VerifyModel();
		}

        public void SetEvent(EventInfo @event)
		{
            _assembly.AssemblyModel.Event = FindEvent(@event);
			VerifyModel();
		}

        public void SetEvent(EventDef @event)
		{
            _assembly.AssemblyModel.Event = @event;
			VerifyModel();
		}

        private TypeDef FindType(Type t)
        {
            foreach(var type in _assembly.AssemblyData.Module.Types)
            {
                foreach(var nestedType in type.NestedTypes)
                {
                    if(Compare(nestedType, t))
                    {
                        return nestedType;
                    }
                }
				if (Compare(type, t))
				{
                    return type;
				}
            }
            return null;
        }

        private MethodDef FindMethod(MethodInfo m)
        {
            foreach (var method in _assembly.AssemblyModel.Type.Methods)
			{
				if (Compare(method, m))
				{
                    return method;
				}
			}
			return null;
        }

        private PropertyDef FindProperty(PropertyInfo p)
		{
            foreach (var property in _assembly.AssemblyModel.Type.Properties)
			{
                if (Compare(property, p))
				{
                    return property;
				}
			}
			return null;
		}

        private EventDef FindEvent(EventInfo e)
        {
            foreach (var @event in _assembly.AssemblyModel.Type.Events)
			{
				if (Compare(@event, e))
				{
					return @event;
				}
			}
			return null;
        }

        private FieldDef FindField(FieldInfo f)
        {
            foreach (var field in _assembly.AssemblyModel.Type.Fields)
			{
				if (Compare(field, f))
				{
                    return field;
				}
			}
			return null;
        }

        private bool Compare(dynamic one, dynamic two)
        {
            return new SigComparer().Equals(one, two);
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
