using System;

namespace DISL.Runtime.Bindings
{
    internal interface IBindingProcessor
    {
        void Register(Type bindingType);
        void SetImplementation(Type bindingType, Type implementationType);

        void SetSingle(Type bindingType);
        void SetNonLazy(Type bindingType);
        void SetInstance(Type bindingType, object instance);
        
        void Unregister(Type bindingType);
    }
}