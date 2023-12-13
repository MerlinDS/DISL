using System;
using DISL.Runtime.Bindings;
using JetBrains.Annotations;

namespace DISL.Runtime.Base
{
    public interface IContainer : IBindingContainer, IResolvingContainer, IDisposable
    {
        /// <summary>
        /// The name of the container.
        /// </summary>
        [CanBeNull]
        string Name { get; }

        /// <summary>
        /// The parent container.
        /// </summary>
        [CanBeNull]
        IContainer Parent { get; }
    }
}