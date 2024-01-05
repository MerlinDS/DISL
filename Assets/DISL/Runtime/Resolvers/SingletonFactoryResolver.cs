using System;
using DISL.Runtime.Base;
using DISL.Runtime.Generics;

namespace DISL.Runtime.Resolvers
{
    internal sealed class SingletonFactoryResolver : IResolver
    {
        private object _instance;
        private readonly Func<IContainer, object> _factory;
        private readonly DisposableCollection _disposables = new();

        public SingletonFactoryResolver(Func<IContainer, object> factory)
        {
            _factory = factory;
        }

        /// <inheritdoc />
        public object Resolve(IContainer container)
        {
            if (_instance != null) 
                return _instance;
            
            _instance = _factory.Invoke(container);
            _disposables.TryAdd(_instance);
            return _instance;
        }
        
        /// <inheritdoc />
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}