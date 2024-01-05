using System;
using DISL.Runtime.Base;
using DISL.Runtime.Generics;

namespace DISL.Runtime.Resolvers
{
    internal sealed class TransientTypeResolver : IResolver
    {
        private readonly Type _concreteType;
        private readonly DisposableCollection _disposables = new();
        
        public TransientTypeResolver(Type concreteType)
        {
            _concreteType = concreteType;
        }
        
        /// <inheritdoc />
        public object Resolve(IContainer container)
        {
            var instance = container.Construct(_concreteType);
            _disposables.TryAdd(instance);
            return instance;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}