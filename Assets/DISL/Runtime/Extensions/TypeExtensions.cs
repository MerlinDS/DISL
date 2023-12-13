using System;
using System.Reflection;

namespace DISL.Runtime.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool TryGetConstructors(this Type type, out ConstructorInfo[] constructors)
        {
            constructors = type.GetConstructors();
            return constructors.Length > 0;
        }   
    }
}