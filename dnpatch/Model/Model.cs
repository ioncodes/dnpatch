using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dnlib.DotNet;
using dnpatch.Enums;
using Assembly = dnpatch.Types.Assembly;

namespace dnpatch.Model
{
    /// <summary>
    /// The model for the assembly
    /// </summary>
    public class Model
    {
        /// <summary>
        /// The assembly
        /// </summary>
        protected Assembly Assembly;

        internal Model(Assembly assembly)
        {
            Assembly = assembly;
        }

        /// <summary>
        /// Sets the target namespace.
        /// </summary>
        /// <param name="namespace">The namespace.</param>
        public void SetNamespace(string @namespace) // TODO: Remove this, I never needed it...
		{
			Assembly.AssemblyModel.Namespace = @namespace;
		}

        /// <summary>
        /// Sets the target type.
        /// </summary>
        /// <param name="classPath">The class path.</param>
        /// <exception cref="Exception"></exception>
        public void SetType(string classPath)
		{
			string path = $"{Assembly.AssemblyModel.Namespace}.{classPath}";
			TypeDef type = Assembly.AssemblyData.Module.FindReflection(path);
			Assembly.AssemblyModel.Type = type ?? throw new Exception($"Type '{path}' does not exist.");
		}

        /// <summary>
        /// Sets the target type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetType(Type type)
		{
            Assembly.AssemblyModel.Type = FindType(type);
		}

        /// <summary>
        /// Sets the target type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetType(TypeDef type)
		{
            Assembly.AssemblyModel.Type = type;
		}

        /// <summary>
        /// Sets the target field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <exception cref="Exception"></exception>
        public void SetField(string fieldName)
		{
			Assembly.AssemblyModel.Field = Assembly.AssemblyModel.Type.FindField(fieldName) ?? throw new Exception($"Field '{Assembly.AssemblyModel.Type.FullName}.{fieldName}' does not exist.");
			VerifyModel();
		}

        /// <summary>
        /// Sets the target field.
        /// </summary>
        /// <param name="field">The field.</param>
        public void SetField(FieldInfo field)
		{
            Assembly.AssemblyModel.Field = FindField(field);
			VerifyModel();
		}

        /// <summary>
        /// Sets the target field.
        /// </summary>
        /// <param name="field">The field.</param>
        public void SetField(FieldDef field)
		{
            Assembly.AssemblyModel.Field = field;
			VerifyModel();
		}

        /// <summary>
        /// Sets the target method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <exception cref="Exception"></exception>
        public void SetMethod(string methodName)
		{
			Assembly.AssemblyModel.Method = Assembly.AssemblyModel.Type.FindMethod(methodName) ?? throw new Exception($"Method '{Assembly.AssemblyModel.Type.FullName}.{methodName}' does not exist.");
			VerifyModel();
		}

        /// <summary>
        /// Sets the target method.
        /// </summary>
        /// <param name="method">The method.</param>
        public void SetMethod(MethodInfo method)
		{
            Assembly.AssemblyModel.Method = FindMethod(method);
			VerifyModel();
		}

        /// <summary>
        /// Sets the target method.
        /// </summary>
        /// <param name="method">The method.</param>
        public void SetMethod(MethodDef method)
		{
            Assembly.AssemblyModel.Method = method;
			VerifyModel();
		}

        /// <summary>
        /// Sets the target property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyMethod">The property method.</param>
        /// <exception cref="Exception"></exception>
        public void SetProperty(string propertyName, PropertyMethod propertyMethod)
		{
			Assembly.AssemblyModel.Property = Assembly.AssemblyModel.Type.FindProperty(propertyName) ?? throw new Exception($"Property '{Assembly.AssemblyModel.Type.FullName}.{propertyName}' does not exist.");
			Assembly.AssemblyModel.PropertyMethod = propertyMethod;
			VerifyModel();
		}

        /// <summary>
        /// Sets the target property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void SetProperty(PropertyInfo property)
		{
            Assembly.AssemblyModel.Property = FindProperty(property);
			VerifyModel();
		}

        /// <summary>
        /// Sets the target property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void SetProperty(PropertyDef property)
		{
            Assembly.AssemblyModel.Property = property;
			VerifyModel();
		}

        /// <summary>
        /// Sets the target event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <exception cref="Exception"></exception>
        public void SetEvent(string eventName)
		{
			Assembly.AssemblyModel.Event = Assembly.AssemblyModel.Type.FindEvent(eventName) ?? throw new Exception($"Event '{Assembly.AssemblyModel.Type.FullName}.{eventName}' does not exist.");
			VerifyModel();
		}

        /// <summary>
        /// Sets the target event.
        /// </summary>
        /// <param name="event">The event.</param>
        public void SetEvent(EventInfo @event)
		{
            Assembly.AssemblyModel.Event = FindEvent(@event);
			VerifyModel();
		}

        /// <summary>
        /// Sets the target event.
        /// </summary>
        /// <param name="event">The event.</param>
        public void SetEvent(EventDef @event)
		{
            Assembly.AssemblyModel.Event = @event;
			VerifyModel();
		}

        private TypeDef FindType(Type t)
        {
            foreach(var type in Assembly.AssemblyData.Module.Types)
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
            foreach (var method in Assembly.AssemblyModel.Type.Methods)
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
            foreach (var property in Assembly.AssemblyModel.Type.Properties)
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
            foreach (var @event in Assembly.AssemblyModel.Type.Events)
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
            foreach (var field in Assembly.AssemblyModel.Type.Fields)
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
				Assembly.AssemblyModel.Event != null,
				Assembly.AssemblyModel.Field != null,
				Assembly.AssemblyModel.Method != null,
				Assembly.AssemblyModel.Property != null
			}.Count(b => b) > 1) throw new Exception($"Check your AssemblyModel in Assembly '{Assembly.AssemblyInfo.InternalName}'. Multiple assignments found for properties: Event, Field, Method, Property");
		}
    }
}
