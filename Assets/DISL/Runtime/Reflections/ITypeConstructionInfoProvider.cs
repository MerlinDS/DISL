using System;

namespace DISL.Runtime.Reflections
{
    public interface ITypeConstructionInfoProvider
    {
        TypeConstructionInfo Get(Type type);
    }
}