using System;

namespace DISL.Runtime.Bindings
{
    public readonly struct BindingDescriptor
    {
        public BindingDescriptor(Type implementationType, bool isSingle = false, bool isLazy = true)
        {
            ImplementationType = implementationType;
            IsSingle = isSingle;
            IsLazy = isLazy;
        }

        public BindingDescriptor ToSingle() => new(ImplementationType, true, IsLazy);
        public BindingDescriptor ToNonLazy() => new(ImplementationType, IsSingle, false);

        public BindingDescriptor ChangeImplementation(Type implementationType) =>
            new(implementationType, IsSingle, IsLazy);

        public Type ImplementationType { get; }

        public bool IsSingle { get; }
        public bool IsLazy { get; }

        public bool IsNonLazy => !IsLazy;
    }
}