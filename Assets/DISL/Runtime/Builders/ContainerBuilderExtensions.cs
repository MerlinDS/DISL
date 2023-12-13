using System;
using DISL.Runtime.Base;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Reflections;
using DISL.Runtime.Utils;

namespace DISL.Runtime.Builders
{
    public static partial class ContainerBuilder
    {
        /// <summary>
        /// Sets the name of the container
        /// </summary>
        /// <param name="builder">The container builder</param>
        /// <param name="name">The name to set</param>
        /// <returns>The container builder with the name property set</returns>
        public static IContainerBuilder WithName(this IContainerBuilder builder, string name) =>
            builder.GetSkeleton().SetProperty(name);

        /// <summary>
        /// Configures the parent container for a given container.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="parent">The parent container.</param>
        /// <returns>The container builder with the parent container configured.</returns>
        public static IContainerBuilder WithParent(this IContainerBuilder builder, IContainer parent) =>
            builder.GetSkeleton().SetProperty(parent);

        /// <summary>
        /// Adds a <see cref="ScriptingBackend"/> to the container builder and returns the modified container builder.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="scriptingBackend">The scripting backend to be added.</param>
        /// <returns>The modified container builder.</returns>
        public static IContainerBuilder For(this IContainerBuilder builder, ScriptingBackend scriptingBackend) =>
            builder.GetSkeleton().SetProperty(scriptingBackend);

        /// <summary>
        /// Sets the <see cref="IActivatorFactoriesResolver"/> to the container builder and returns the modified container builder.
        /// </summary>
        /// <param name="builder">The container builder to modify.</param>
        /// <param name="resolver">The activator factories resolver to set.</param>
        /// <returns>The modified container builder.</returns>
        public static IContainerBuilder With(this IContainerBuilder builder, IActivatorFactoriesResolver resolver) =>
            builder.GetSkeleton().SetProperty(resolver);

        /// <summary>
        /// Sets a custom property to the container builder.
        /// </summary>
        /// <param name="builder">The container builder.</param>
        /// <param name="value">The value to set.</param>
        /// <typeparam name="T">The type of the property to set.</typeparam>
        /// <returns>The updated container builder.</returns>
        public static IContainerBuilder WithProperty<T>(this IContainerBuilder builder, T value) =>
            builder.GetSkeleton().SetProperty(value);

        /// <summary>
        /// Retrieves the <see cref="ITypeConstructionInfoProvider"/> from the given <see cref="IContainerBuilder"/>.
        /// </summary>
        /// <param name="builder">The container builder from which to retrieve the provider.</param>
        /// <returns>The <see cref="ITypeConstructionInfoProvider"/> if it is defined in the container builder.</returns>
        /// <exception cref="DISLException">Thrown when the <see cref="ITypeConstructionInfoProvider"/> is not defined. Build the container first.</exception>
        public static ITypeConstructionInfoProvider GetProvider(this IContainerBuilder builder)
        {
            var property = builder.GetSkeleton().GetProperty<ITypeConstructionInfoProvider>();
            if (property is not null)
                return property;

            throw new DISLException("TypeConstructionInfoProvider is not defined. Build container first.");
        }

        /// <summary>
        /// Builds an instance of a bindings collection.
        /// </summary>
        /// <param name="builder">The container builder to build the collection from.</param>
        /// <returns>The built instance of the bindings collection.</returns>
        /// <exception cref="DISLException">
        /// Thrown when the bindings collection is not defined. Build the container first.
        /// </exception>
        public static IBindingsCollection BuildCollection(this IContainerBuilder builder)
        {
            var property = builder.GetSkeleton().GetProperty<BindingsCollectionFactory>();
            if (property is not null)
                return property.Invoke();

            throw new DISLException("BindingsCollection is not defined. Build container first.");
        }

        /// <summary>
        /// Builds and returns an instance of IContainer using the specified IContainerBuilder.
        /// </summary>
        /// <param name="builder">The IContainerBuilder used to build the container.</param>
        /// <returns>An instance of IContainer.</returns>
        internal static IContainer Build(this IContainerBuilder builder)
        {
            var skeleton = builder.Prepare();
            var type = skeleton.Type;

            var info = skeleton.GetProvider().Get(type);
            var arguments = skeleton.GetArguments(info.Parameters);
            try
            {
                return (IContainer)info.Activator.Invoke(arguments);
            }
            catch (Exception e)
            {
                throw new DISLInvalidConstructionException(type, e);
            }
        }

        private static IContainerSkeleton Prepare(this IContainerBuilder skeleton) =>
            skeleton.GetSkeleton().BuildBindingsCollectionFactory().BuildTypeConstructionInfoProvider();

        private static IContainerSkeleton BuildBindingsCollectionFactory(this IContainerSkeleton skeleton)
        {
            var collectionFactory = skeleton.GetProperty<BindingsCollectionFactory>();
            if (collectionFactory is not null)
                return skeleton;

            collectionFactory = () => new BindingsCollection();
            skeleton = skeleton.With(collectionFactory).GetSkeleton();
            return skeleton;
        }

        private static IContainerSkeleton BuildTypeConstructionInfoProvider(this IContainerSkeleton skeleton)
        {
            if (skeleton.GetProperty<ITypeConstructionInfoProvider>() is not null)
                return skeleton;

            var scriptingBackend = skeleton.GetProperty<ScriptingBackend>();
            if (scriptingBackend == ScriptingBackend.Undefined)
            {
                scriptingBackend = ScriptingBackendResolver.Resolve();
                skeleton = skeleton.SetProperty(scriptingBackend);
            }

            var resolver = skeleton.GetProperty<IActivatorFactoriesResolver>();
            if (resolver is null)
            {
                resolver = ActivatorFactoriesResolver.Default();
                skeleton = skeleton.With(resolver).GetSkeleton();
            }

            var factory = resolver.Resolve(scriptingBackend);
            return skeleton.With(new TypeConstructionInfoProvider(factory)).GetSkeleton();
        }

        internal static IContainerBuilder Clone(this IContainerBuilder builder) =>
            builder.GetSkeleton().Clone();

        internal static IContainerBuilder With(this IContainerBuilder builder, BindingsCollectionFactory factory) =>
            builder.GetSkeleton().SetProperty(factory);

        private static IContainerBuilder With(this IContainerBuilder builder, ITypeConstructionInfoProvider provider)
        {
            return builder.GetSkeleton().SetProperty(provider);
        }
    }
}