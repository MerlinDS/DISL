using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace DISL.Runtime.Bindings
{
    /// <summary>
    /// API for binding configuration.
    /// </summary>
    /// <typeparam name="TType">The type of the binding.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
    public readonly ref struct BindingConfigure<TType, TImplementation>
        where TImplementation : TType, new()
    {
        private readonly Action<Type> _setSingle;
        private readonly Action<Type> _setNonLazy;
        private readonly Action<Type, object> _toInstance;

        private readonly Type _bindingType;

        internal BindingConfigure([NotNull] Action<Type> setSingle, [NotNull] Action<Type> setNonLazy,
            [NotNull] Action<Type, object> toInstance)
        {
            _setSingle = setSingle;
            _setNonLazy = setNonLazy;
            _toInstance = toInstance;

            _bindingType = typeof(TType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingConfigure<TType, TImplementation> AsSingle()
        {
            _setSingle?.Invoke(_bindingType);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingConfigure<TType, TImplementation> NonLazy()
        {
            _setNonLazy?.Invoke(_bindingType);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ToInstance(TImplementation instance)
        {
            _toInstance?.Invoke(_bindingType, instance);
        }
    }
}