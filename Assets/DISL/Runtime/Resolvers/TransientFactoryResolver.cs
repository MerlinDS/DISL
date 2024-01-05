using System;
using DISL.Runtime.Base;
using DISL.Runtime.Generics;

namespace DISL.Runtime.Resolvers
{
    internal sealed class TransientFactoryResolver : IResolver
    {
        private readonly Func<IContainer, object> _factory;
        private readonly DisposableCollection _disposables = new();
        public TransientFactoryResolver(Func<IContainer, object> factory)
        {
            _factory = factory;
        }
    
        public object Resolve(IContainer container)
        {
            var instance = _factory.Invoke(container);
            _disposables.TryAdd(instance);
            return instance;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}