using System;
using System.Collections.Generic;
using System.Linq;

namespace DISL.Runtime.Reflections
{
    internal sealed class TypeConstructionInfoProvider : ITypeConstructionInfoProvider
    {
        private readonly IActivatorFactory _factory;
        private readonly Dictionary<Type, TypeConstructionInfo> _dictionary = new();

        public TypeConstructionInfoProvider(IActivatorFactory factory)
        {
            _factory = factory;
        }

        public TypeConstructionInfo Get(Type type)
        {
            if (_dictionary.TryGetValue(type, out var info))
                return info;

            info = Generate(type);
            _dictionary.Add(type, info);
            return info;
        }

        private TypeConstructionInfo Generate(Type type)
        {
            var constructors = type.GetConstructors();
            
            if(constructors.Length == 0)
                return new TypeConstructionInfo(_factory.GenerateDefaultActivator(type), Type.EmptyTypes);
            
            var constructor = constructors.OrderByDescending(ctor => ctor.GetParameters().Length).First();
            var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            var activator = _factory.GenerateActivator(type, constructor, parameters);
            return new TypeConstructionInfo(activator, parameters);
        }
    }
}