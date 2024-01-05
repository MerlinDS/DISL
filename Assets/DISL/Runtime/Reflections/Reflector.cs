using System;
using System.Collections.Generic;
using System.Linq;
using DISL.Runtime.Base;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Utils;

namespace DISL.Runtime.Reflections
{
    public class Reflector : IConstructor
    {
        private readonly IActivatorFactory _activatorFactory;
        private readonly Dictionary<Type, TypeConstructionInfo> _dictionary = new();

        public Reflector(IActivatorFactory activatorFactory)
        {
            _activatorFactory = activatorFactory;
        }

        /// <inheritdoc />
        object IConstructor.Construct(Type type, Func<Type, object> resolver)
        {
            var info = GetConstructionInfo(type);
            var length = info.Parameters.Length;

            var arguments = ExactArrayPool<object>.Shared.Rent(length);
            try
            {
                for (var index = 0; index < length; index++)
                {
                    var parameterType = info.Parameters[index];
                    arguments[index] = resolver?.Invoke(parameterType);
                }

                return info.Activator.Invoke(arguments);
            }
            catch (Exception inner)
            {
                //Activator can throw any exception
                throw new DISLInvalidConstructionException(type, inner);
            }
            finally
            {
                ExactArrayPool<object>.Shared.Return(arguments);
            }
        }

        private TypeConstructionInfo GetConstructionInfo(Type type)
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

            if (constructors.Length == 0)
                return new TypeConstructionInfo(_activatorFactory.GenerateDefaultActivator(type), Type.EmptyTypes);

            var constructor = constructors.OrderByDescending(ctor => ctor.GetParameters().Length).First();
            var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            var activator = _activatorFactory.GenerateActivator(type, constructor, parameters);
            return new TypeConstructionInfo(activator, parameters);
        }
    }
}