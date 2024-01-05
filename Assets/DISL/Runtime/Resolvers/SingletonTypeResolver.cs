using System;
using DISL.Runtime.Base;
using DISL.Runtime.Generics;

namespace DISL.Runtime.Resolvers
{
    internal sealed class SingletonTypeResolver : IResolver
    {
        private object _instance;
        private readonly Type _concreteType;
        private readonly DisposableCollection _disposables = new();
        
        public SingletonTypeResolver(Type concreteType)
        {
            _concreteType = concreteType;
        }
        /// <inheritdoc />
        public object Resolve(IContainer container)
        {
            if (_instance != null) 
                return _instance;
            
            _instance = container.Construct(_concreteType);
            _disposables.TryAdd(_instance);
            return _instance;
        }

        /// <inheritdoc />
        public void Dispose() => 
            _disposables.Dispose();
    }
}