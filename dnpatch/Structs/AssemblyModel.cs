using dnlib.DotNet;
using dnpatch.Enums;

namespace dnpatch.Structs
{
    // The good ol' Target
    public struct AssemblyModel
    {
        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>
        /// The target namespace.
        /// </value>
        public string Namespace { get; internal set; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The target type.
        /// </value>
        public TypeDef Type { get; internal set; }
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>
        /// The target method.
        /// </value>
        public MethodDef Method { get; internal set; }
        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>
        /// The target field.
        /// </value>
        public FieldDef Field { get; internal set; }
        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The target property.
        /// </value>
        public PropertyDef Property { get; internal set; }
        /// <summary>
        /// Gets the property method.
        /// </summary>
        /// <value>
        /// The target property method.
        /// </value>
        public PropertyMethod PropertyMethod { get; internal set; }
        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <value>
        /// The target event.
        /// </value>
        public EventDef Event { get; internal set; }
    }
}
