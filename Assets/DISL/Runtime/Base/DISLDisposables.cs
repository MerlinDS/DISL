using System;
using System.Collections.Generic;

namespace DISL.Runtime.Base
{
    internal sealed class DISLDisposables : IDisposable
    {
        private readonly Queue<IDisposable> _disposables = new();
        
        public void Add(IDisposable disposable)
        {
            _disposables.Enqueue(disposable);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}