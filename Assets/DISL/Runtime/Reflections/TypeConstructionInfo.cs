using System;
using DISL.Runtime.Delegates;

namespace DISL.Runtime.Reflections
{
    public sealed class TypeConstructionInfo
    {
        public ObjectActivator Activator { get; }
        public Type[] Parameters { get; }
            
        public TypeConstructionInfo(ObjectActivator activator, Type[] parameters)
        {
            Activator = activator;
            Parameters = parameters;
        }
    }
}