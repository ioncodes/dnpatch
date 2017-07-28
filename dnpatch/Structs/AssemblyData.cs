using System.Collections.Generic;
using dnlib.DotNet;

namespace dnpatch.Structs
{
    public struct AssemblyData
    {
        /// <summary>
        /// Gets the module.
        /// </summary>
        /// <value>
        /// The assembly module.
        /// </value>
        public ModuleDefMD Module { get; internal set; }
        /// <summary>
        /// Gets the entrypoint.
        /// </summary>
        /// <value>
        /// The assembly entrypoint.
        /// </value>
        public MethodDef Entrypoint { get; internal set; }
        /// <summary>
        /// Gets the importer.
        /// </summary>
        /// <value>
        /// dnlib's Importer
        /// </value>
        public Importer Importer { get; internal set; }
        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <value>
        /// All assembly methods.
        /// </value>
        public List<MethodDef> Methods { get; internal set; }
        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <value>
        /// All assembly types.
        /// </value>
        public List<TypeDef> Types { get; internal set; }
    }
}
