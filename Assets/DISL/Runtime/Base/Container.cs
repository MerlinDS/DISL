using System;
using DISL.Runtime.Bindings;
using DISL.Runtime.Builders;
using DISL.Runtime.Reflections;

namespace DISL.Runtime.Base
{
    internal sealed class Container : BaseContainer
    {

        private readonly IBindingsCollection _collection;
        private readonly ITypeConstructionInfoProvider _typeConstructionInfoProvider;

        public Container(string name, IContainer parent, IContainerBuilder builder) : base(name, parent, builder)
        {
            _collection = builder.BuildCollection();
            _typeConstructionInfoProvider = builder.GetProvider();
        }
        
        public override Binding<T> Bind<T>()
        {
            // return new Binding<T>(_bindingProcessor);
            throw new NotImplementedException();
        }

        //TODO: Move to the base class
        public override T Resolve<T>()
        {
            throw new NotImplementedException();
        }
    }
}