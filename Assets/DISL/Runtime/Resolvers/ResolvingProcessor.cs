using System;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Bindings;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Reflections;
using DISL.Runtime.Utils;
using JetBrains.Annotations;

namespace DISL.Runtime.Resolvers
{
    internal sealed class ResolvingProcessor : BaseProcessor, IResolvingProcessor
    {
        [CanBeNull] private readonly IResolvingProcessor _parent;

        private readonly ITypeConstructionInfoProvider _typeConstructionInfoProvider;

        public ResolvingProcessor([CanBeNull] IResolvingProcessor parent,
            ITypeConstructionInfoProvider typeConstructionInfoProvider,
            IDictionary<Type, BindingDescriptor> bindings, IDictionary<Type, object> instances)
            : base(bindings, instances)
        {
            _parent = parent;
            _typeConstructionInfoProvider = typeConstructionInfoProvider;
        }

        /// <inheritdoc />
        public object Resolve(Type type)
        {
            if (!Has(type) && _parent is not null)
                return _parent.Resolve(type);

            return ResolveOrCreateInstance(type);
        }

        private object ResolveOrCreateInstance(Type type)
        {
            var descriptor = GetDescriptor(type);
            return descriptor.IsSingle ? ResolveSingle(type, descriptor) : CreateInstance(type, descriptor);
        }

        private object ResolveSingle(Type type, in BindingDescriptor descriptor)
        {
            if (Instances.TryGetValue(type, out var instance) && instance is not null)
                return instance;

            instance = CreateInstance(type, descriptor);
            Instances[type] = instance;

            return instance;
        }

        private object CreateInstance(Type type, BindingDescriptor descriptor)
        {
            var instance = Construct(descriptor.ImplementationType);
            NotifyInstanceCreation(type, instance);
            return instance;
        }

        private void NotifyInstanceCreation(Type type, object instance)
        {
            if (instance is null)
                return;
 
            //TODO: Add notification
        }

        private object Construct(Type type)
        {
            var info = _typeConstructionInfoProvider.Get(type);
            var length = info.Parameters.Length;

            var arguments = ExactArrayPool<object>.Shared.Rent(length);
            try
            {
                for (var index = 0; index < length; index++)
                {
                    var parameterType = info.Parameters[index];
                    arguments[index] = Resolve(parameterType);
                }

                return info.Activator.Invoke(arguments);
            }
            catch (DISLBindingRegistrationException inner)
            {
                //Resolve can throw DISLBindingRegistrationException
                throw new DISLInvalidConstructionException(type, inner);
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
    }
}