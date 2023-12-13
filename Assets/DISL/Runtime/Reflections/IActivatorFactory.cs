using System;
using System.Reflection;
using DISL.Runtime.Delegates;

namespace DISL.Runtime.Reflections
{
    public interface IActivatorFactory
    {
        ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters);
        ObjectActivator GenerateDefaultActivator(Type type);
    }
}