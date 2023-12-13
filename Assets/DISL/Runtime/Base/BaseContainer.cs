using System;
using DISL.Runtime.Bindings;
using DISL.Runtime.Builders;

namespace DISL.Runtime.Base
{
    /// <summary>
    /// The abstract base class for containers.
    /// Provides the basic contract implementation.
    /// </summary>
    public abstract class BaseContainer : IContainer
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IContainer Parent { get; }

        protected IContainerBuilder Builder { get; }

        protected BaseContainer(string name, IContainer parent, IContainerBuilder builder)
        {
            Name = name;
            Parent = parent;
            Builder = builder;
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            
        }

        /// <inheritdoc />
        public virtual Binding<T> Bind<T>()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public virtual T Resolve<T>()
        {
            throw new NotImplementedException();
        }
    }
}