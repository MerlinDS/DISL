using System;
using System.Collections.Generic;
using DISL.Runtime.Bindings;

namespace DISL.Runtime.Base
{
    internal sealed class BindingsCollection : IBindingsCollection
    {
        private readonly IDictionary<Type, BindingDescriptor> _descriptors;
        private readonly IDictionary<Type, object> _instances;

        public BindingsCollection() :
            this(new Dictionary<Type, BindingDescriptor>(), new Dictionary<Type, object>())
        {
        }

        internal BindingsCollection(IDictionary<Type, BindingDescriptor> descriptors,
            IDictionary<Type, object> instances)
        {
            _descriptors = descriptors;
            _instances = instances;
        }

        /// <inheritdoc />
        public void Add(Type bindingType, BindingDescriptor descriptor)
        {
            if (Has(bindingType))
                throw new ArgumentException($"Type {bindingType} already added.", nameof(bindingType));
            
            _descriptors.Add(bindingType, descriptor);
            _instances.Add(bindingType, null);
        }

        /// <inheritdoc />
        public bool Has(Type bindingType) =>
            _descriptors.ContainsKey(bindingType);


        /// <inheritdoc />
        public void Remove(Type bindingType)
        {
            ThrowIfTypeNotFound(bindingType);
            _descriptors.Remove(bindingType);
            _instances.Remove(bindingType);
        }

        /// <inheritdoc />
        public void SetInstance(Type bindingType, object instance)
        {
            ThrowIfTypeNotFound(bindingType);
            _instances[bindingType] = instance;
        }

        /// <inheritdoc />
        public object GetInstance(Type bindingType)
        {
            ThrowIfTypeNotFound(bindingType);
            return _instances[bindingType];
        }

        /// <inheritdoc />
        public void SetDescriptor(Type bindingType, BindingDescriptor descriptor)
        {
            ThrowIfTypeNotFound(bindingType);
            _descriptors[bindingType] = descriptor;
        }


        /// <inheritdoc />
        public BindingDescriptor GetDescriptor(Type bindingType)
        {
            ThrowIfTypeNotFound(bindingType);
            return _descriptors[bindingType];
        }
        
        private void ThrowIfTypeNotFound(Type bindingType)
        {
            if (!Has(bindingType))
                throw new KeyNotFoundException($"Type {bindingType} not found.");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _descriptors.Clear();
            _instances.Clear();
        }
    }
}