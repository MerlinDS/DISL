using DISL.Runtime.Base;
using DISL.Runtime.Generics;

namespace DISL.Runtime.Resolvers
{
    internal sealed class SingletonValueResolver : IResolver
    {
        private readonly object _value;
        private readonly DisposableCollection _disposables = new();
        
        public SingletonValueResolver(object value)
        {
            _value = value;
            _disposables.TryAdd(value);
        }
        
        /// <inheritdoc />
        public object Resolve(IContainer container)
        {
            return _value;
        }
        
        
        /// <inheritdoc />
        public void Dispose()
        {
            _disposables.Dispose();
        }

    }
}