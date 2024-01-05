using System;
using System.Collections.Generic;

namespace DISL.Runtime.Base
{
    internal sealed class Container : IContainer
    {
        private readonly IContainer _parent;
        private readonly Dictionary<string, IContainer> _children = new(1);

        private readonly Dictionary<Type, Binding> _bindings = new();

        private readonly IConstructor _constructor;

        public string Name { get; }

        internal Container(string name, IConstructor constructor)
        {
            Name = name;
            _constructor = constructor;
        }

        private Container(string name, IContainer parent) : this(name, (IConstructor)parent)
        {
            _parent = parent;
        }

        void IContainer.CreateChild(string name)
        {
            var child = new Container(name, this);
            _children.Add(name, child);
        }

        public T Resolve<T>() =>
            (T)Resolve(typeof(T));

        public object Resolve(Type type)
        {
            if(type == typeof(IContainer))
                return this;
            
            if (_bindings.TryGetValue(type, out var binding))
                return binding.Resolver.Resolve(this);

            if (_parent is not null)
                return _parent.Resolve(type);

            throw new ArgumentException($"Binding for type {type} not found");
        }

        /// <inheritdoc />
        public bool HasChild(string name) =>
            _children.ContainsKey(name);

        /// <inheritdoc />
        void IContainer.AddBinding(Binding binding)
        {
            foreach (var contract in binding.Contracts)
                _bindings.Add(contract, binding);
        }


        /// <inheritdoc />
        public IContainer this[string name]
        {
            get
            {
                if (_children.TryGetValue(name, out var container))
                    return container;

                throw new ArgumentException($"Container with name {name} not found");
            }
        }

        object IConstructor.Construct(Type concrete, Func<Type, object> resolver)
        {
            return _constructor.Construct(concrete, resolver ?? Resolve);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var child in _children.Values)
                child.Dispose();
            foreach (var binding in _bindings) 
                binding.Value.Dispose();
            _bindings.Clear();
        }
    }
}