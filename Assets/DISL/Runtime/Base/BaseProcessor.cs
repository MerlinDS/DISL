using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DISL.Runtime.Bindings;
using DISL.Runtime.Exceptions;

namespace DISL.Runtime.Base
{
    internal abstract class BaseProcessor
    {
        internal BaseProcessor(IDictionary<Type, BindingDescriptor> bindings, IDictionary<Type, object> instances)
        {
            Bindings = bindings;
            Instances = instances;
        }

        protected IDictionary<Type, BindingDescriptor> Bindings { get; }
        protected IDictionary<Type, object> Instances { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected BindingDescriptor GetDescriptor(Type bindingType)
        {
            ThrowIfBindingNotExists(bindingType);
            return Bindings[bindingType];
        }

        protected bool Has(Type bindingType) =>
            Bindings.ContainsKey(bindingType);

        protected void ThrowIfBindingExists(Type bindingType)
        {
            if (Has(bindingType))
            {
                throw new DISLBindingRegistrationException($"Binding for type {bindingType} already registered.");
            }
        }

        protected void ThrowIfBindingNotExists(Type bindingType)
        {
            if (!Has(bindingType))
            {
                throw new DISLBindingRegistrationException($"Binding for type {bindingType} not registered.");
            }
        }
    }
}