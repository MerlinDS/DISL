using System;

namespace DISL.Runtime.Bindings
{
    /// <summary>
    /// Binding API.
    /// </summary>
    /// <typeparam name="TType">The type of the binding.</typeparam>
    public readonly ref struct Binding<TType>
    {
        private readonly Action<Type> _setSingle;
        private readonly Action<Type> _setNonLazy;
        private readonly Action<Type, object> _toInstance;
        private readonly Action<Type, Type> _setImplementation;

        private readonly Type _bindingType;

        internal Binding(IBindingProcessor processor)
        {
            _bindingType = typeof(TType);

            if (processor is null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            _setSingle = processor.SetSingle;
            _setNonLazy = processor.SetNonLazy;
            _toInstance = processor.SetInstance;
            _setImplementation = processor.SetImplementation;
        }

        /// <summary>
        /// Sets the binding type to implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns><see cref="BindingConfigure{TType,TImplementation}"> Binding configuration API </see></returns>
        public BindingConfigure<TType, TImplementation> To<TImplementation>()
            where TImplementation : TType, new()
        {
            _setImplementation(_bindingType, typeof(TImplementation));
            return new BindingConfigure<TType, TImplementation>(_setSingle, _setNonLazy, _toInstance);
        }
    }
}