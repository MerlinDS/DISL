using System;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Resolvers;

namespace DISL.Runtime.Builders
{
    public static class ContainerBuilderExtensions
    {
        internal static IContainerBuilder Create(string name)
        {
            return new ContainerBuilderSkeleton
            {
                Name = name,
            };
        }

        /// <summary>
        /// Get or create a child container builder
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="name">Child name</param>
        /// <returns></returns>
        public static IContainerBuilder GetScope(this IContainerBuilder builder, string name)
        {
            var child = Create(name);
            var skeleton = GetSkeleton(builder);
            skeleton.Children.Add((IContainerBuilderSkeleton)child);
            return child;
        }

        /// <summary>
        /// Bind a type to the container
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <typeparam name="T"> Type to bind</typeparam>
        /// <returns></returns>
        public static IContainerBuilder Bind<T>(this IContainerBuilder builder) =>
            new ContractBuilderSkeleton(builder, typeof(T));

        /// <summary>
        /// Bind a type to the container as a singleton
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <typeparam name="T"> Type to bind</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> If type is not concrete</exception>
        public static IContainerBuilder AsSingleton<T>(this IContainerBuilder builder)
        {
            var concrete = typeof(T);
            Validate(concrete);
            var resolver = new SingletonTypeResolver(concrete);
            return builder.AddBinding(resolver, concrete);
        }

        /// <summary>
        /// Bind an instance to the container as a singleton
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="instance">Instance to bind</param>
        /// <typeparam name="T"> Type to bind</typeparam>
        /// <returns></returns>
        public static IContainerBuilder AsSingleton<T>(this IContainerBuilder builder, T instance)
        {
            var resolver = new SingletonValueResolver(instance);
            return builder.AddBinding(resolver, instance.GetType(), typeof(T));
        }
        
        /// <summary>
        /// Bind a factory to the container as a singleton
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="factory">Factory method to bind</param>
        /// <typeparam name="T">Type to bind</typeparam>
        /// <returns></returns>
        public static IContainerBuilder AsSingleton<T>(this IContainerBuilder builder, Func<IContainer, T> factory)
        {
            var resolver = new SingletonFactoryResolver(container => factory.Invoke(container));
            return builder.AddBinding(resolver, typeof(T));
        }

        /// <summary>
        /// Bind a type to the container as a transient
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <typeparam name="T"> Type to bind</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> If type is not concrete</exception>
        public static IContainerBuilder AsTransient<T>(this IContainerBuilder builder)
        {
            var concrete = typeof(T);
            Validate(concrete);
            return builder.AddBinding(new TransientTypeResolver(concrete), concrete);
        }

        /// <summary>
        /// Bind a factory to the container as a transient
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="factory">Factory method to bind</param>
        /// <typeparam name="T">Type to bind</typeparam>
        /// <returns></returns>
        public static IContainerBuilder AsTransient<T>(this IContainerBuilder builder,
            Func<IContainer, T> factory)
        {
            var resolver = new TransientFactoryResolver(container => factory.Invoke(container));
            return builder.AddBinding(resolver, typeof(T));
        }

        private static void Validate(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
                throw new ArgumentException($"Type {type} is not concrete");
        }

        private static IContainerBuilder AddBinding(this IContainerBuilder builder, IResolver resolver,
            Type concrete, params Type[] contracts)
        {
            var contractsList = new List<Type>(contracts);
            if (contractsList.Count == 0)
                contractsList.Add(concrete);

            while (builder is ContractBuilderSkeleton { Builder: not null } contractBuilder)
            {
                contractsList.Add(contractBuilder.Contract);
                builder = contractBuilder.Builder;
            }

            var skeleton = GetSkeleton(builder);
            skeleton.Bindings.Add(Binding.Create(resolver, concrete, contractsList.ToArray()));
            return builder;
        }

        internal static void Build(this IContainerBuilder builder, IContainer container)
        {
            var skeleton = GetSkeleton(builder);
            BuildContainer(container, skeleton.Bindings);

            foreach (var child in skeleton.Children)
            {
                if (!container.HasChild(child.Name))
                    container.CreateChild(child.Name);

                BuildContainer(container[child.Name], child.Bindings);
            }
        }

        private static void BuildContainer(IContainer container, IEnumerable<Binding> bindings)
        {
            foreach (var binding in bindings)
                container.AddBinding(binding);
        }

        private sealed class ContainerBuilderSkeleton : IContainerBuilderSkeleton
        {
            public string Name { get; set; }

            public List<IContainerBuilderSkeleton> Children { get; } = new(1);

            public List<Binding> Bindings { get; } = new();
        }

        private static IContainerBuilderSkeleton GetSkeleton(this IContainerBuilder builder)
        {
            if (builder is IContainerBuilderSkeleton skeleton)
                return skeleton;

            throw new ArgumentException("Container builder is not a skeleton");
        }

        private readonly struct ContractBuilderSkeleton : IContainerBuilder
        {
            public IContainerBuilder Builder { get; }

            public Type Contract { get; }

            public ContractBuilderSkeleton(IContainerBuilder builder, Type contract)
            {
                Builder = builder;
                Contract = contract;
            }
        }
    }
}