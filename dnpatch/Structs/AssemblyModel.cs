using System;
using dnlib.DotNet;

namespace dnpatch
{
    // The good ol' Target
    public struct AssemblyModel
    {
        public string Namespace { get; internal set; }
        public TypeDef Type { get; internal set; }
        public MethodDef Method { get; internal set; }
        public FieldDef Field { get; internal set; }
        public PropertyDef Property { get; internal set; }
        public PropertyMethod PropertyMethod { get; internal set; }
        public EventDef Event { get; internal set; }
    }
}
