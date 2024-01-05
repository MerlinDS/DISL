using System;
using System.Collections.Generic;

namespace DISL.Runtime.Generics
{
    internal sealed class DisposableCollection : IDisposable
    {
        private readonly Stack<IDisposable> _stack = new();

        public DisposableCollection(IEnumerable<IDisposable> disposables = null)
        {
            if (disposables == null)
                return;
            
            foreach (var disposable in disposables)
            {
                _stack.Push(disposable);
            }
        }

        public void Add(IDisposable disposable)
        {
            _stack.Push(disposable);
        }

        public void TryAdd(object obj)
        {
            if (obj is IDisposable disposable)
            {
                _stack.Push(disposable);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            while (_stack.TryPop(out var disposable))
            {
                disposable.Dispose();
            }
        }
    }
}