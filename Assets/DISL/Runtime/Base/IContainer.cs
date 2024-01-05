using System;

namespace DISL.Runtime.Base
{
    public interface IContainer : IConstructor, IDisposable
    {
        string Name { get; }
        T Resolve<T>();
        
        bool HasChild(string name);
        IContainer this[string name] { get; }
        
        object Resolve(Type type);
        internal void CreateChild(string name);
        internal void AddBinding(Binding binding);
    }
}