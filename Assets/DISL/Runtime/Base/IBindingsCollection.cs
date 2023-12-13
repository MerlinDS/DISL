using System;
using System.Collections.Generic;
using DISL.Runtime.Bindings;

namespace DISL.Runtime.Base
{
    public interface IBindingsCollection : IDisposable
    {
        /// <summary>
        /// Adds a binding type and descriptor to the collection.
        /// </summary>
        /// <param name="bindingType">The type of the binding to add.</param>
        /// <param name="descriptor">The descriptor for the binding.</param>
        /// <exception cref="ArgumentException">Thrown if the type is already added.</exception>
        void Add(Type bindingType, BindingDescriptor descriptor);

        /// <summary>
        /// Determines whether the specified binding type exists in the collection.
        /// </summary>
        /// <param name="bindingType">The binding type to check.</param>
        /// <returns>
        /// <c>true</c> if the binding type exists in the collection; otherwise, <c>false</c>.
        /// </returns>
        bool Has(Type bindingType);

        /// <summary>
        /// Removes the specified binding type from the descriptors and instances.
        /// </summary>
        /// <param name="bindingType">The type of the binding to be removed.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the type is not found.</exception>
        void Remove(Type bindingType);

        /// <summary>
        /// Sets an instance of a specified type in the collection.
        /// </summary>
        /// <param name="bindingType">The type of the object being bound.</param>
        /// <param name="instance">The instance object to be set.</param>
        /// <remarks>
        /// This method associates an instance object with a specified type in the internal collection.
        /// If an instance of the same type already exists, it will be replaced with the provided instance.
        /// </remarks>
        /// <exception cref="KeyNotFoundException">Thrown if the type is not found.</exception>
        void SetInstance(Type bindingType, object instance);

        /// <summary>
        /// Retrieves an instance of the specified binding type from the internal dictionary.
        /// </summary>
        /// <param name="bindingType">The type of the object to retrieve.</param>
        /// <returns>
        /// The instance of the specified binding type from the internal dictionary.
        /// </returns>
        /// <exception cref="KeyNotFoundException">Thrown if the type is not found.</exception>
        object GetInstance(Type bindingType);

        /// <summary>
        /// Sets the binding descriptor for a given binding type.
        /// </summary>
        /// <param name="bindingType">The type of the binding.</param>
        /// <param name="descriptor">The descriptor to set.</param>
        /// <exception cref="KeyNotFoundException">Thrown if the type is not found.</exception>
        void SetDescriptor(Type bindingType, BindingDescriptor descriptor);

        /// <summary>
        /// Gets the binding descriptor for the specified binding type.
        /// </summary>
        /// <param name="bindingType">The type of the binding.</param>
        /// <returns>The binding descriptor for the specified binding type.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the type is not found.</exception>
        BindingDescriptor GetDescriptor(Type bindingType);
    }
}