using System;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Exceptions;

namespace DISL.Runtime.Bindings
{
    internal sealed class BindingProcessor : BaseProcessor, IBindingProcessor
    {

        internal BindingProcessor(IDictionary<Type, BindingDescriptor> bindings, IDictionary<Type, object> instances) : 
            base(bindings, instances)
        {
        }

        public void Register(Type bindingType)
        {
            ThrowIfBindingExists(bindingType);
            Bindings.Add(bindingType, new BindingDescriptor(bindingType));
            Instances.Add(bindingType, null);
        }

        /// <inheritdoc />
        public void Unregister(Type bindingType)
        {
            ThrowIfBindingNotExists(bindingType);
            DestroyInstance(bindingType);
            Bindings.Remove(bindingType);
            Instances.Remove(bindingType);
        }

        public void SetImplementation(Type bindingType, Type implementationType)
        {
            var descriptor = GetDescriptor(bindingType);
            DISLBindingImplementationException.Guard(bindingType, implementationType);

            if (descriptor.ImplementationType == implementationType) 
                return;
            
            Bindings[bindingType] = descriptor.ChangeImplementation(implementationType);
            DestroyInstance(bindingType);
        }

        public void SetSingle(Type bindingType)
        {
            Bindings[bindingType] = GetDescriptor(bindingType).ToSingle();
        }

        public void SetNonLazy(Type bindingType)
        {
            Bindings[bindingType] = GetDescriptor(bindingType).ToNonLazy();
        }

        public void SetInstance(Type bindingType, object instance)
        {
            DestroyInstance(bindingType);
            Bindings[bindingType] = GetDescriptor(bindingType).ToSingle();
            Instances[bindingType] = instance;
        }

        private void DestroyInstance(Type type)
        {
            if (Instances[type] is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Instances[type] = null;
        }
    }
}