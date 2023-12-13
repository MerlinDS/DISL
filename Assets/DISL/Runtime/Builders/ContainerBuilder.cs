using System;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Reflections;
using DISL.Runtime.Utils;

namespace DISL.Runtime.Builders
{
    public delegate IBindingsCollection BindingsCollectionFactory();

    public static partial class ContainerBuilder
    {
        /// <summary>
        /// Creates an instance of the default container builder.
        /// </summary>
        /// <returns>
        /// An instance of the default container builder.
        /// </returns>
        public static IContainerBuilder Create() =>
            Create<Container>();

        public static IContainerBuilder Inherit(IContainerBuilder parent, string name) =>
            Inherit<Container>(parent, name);

        public static IContainerBuilder Inherit<T>(IContainerBuilder parent, string name)
            where T : BaseContainer
        {
            var skeleton = Create<T>().GetSkeleton();
            var parentSkeleton = parent.GetSkeleton();
            skeleton.Copy(parentSkeleton);
            skeleton.WithName(name);
            return skeleton;
        }

        /// <summary>
        /// Creates an instance of the container builder for the custom container.
        /// </summary>
        /// <typeparam name="T">The type of the custom container.</typeparam>
        /// <returns>The instance of the container builder for the custom container.</returns>
        public static IContainerBuilder Create<T>() where T : BaseContainer =>
            new ContainerBuilderSkeleton<T>();

        private static IContainerSkeleton GetSkeleton(this IContainerBuilder builder)
        {
            if (builder is not IContainerSkeleton skeleton)
                throw new DISLInvalidBuilderException(builder);
            return skeleton;
        }

        /// <summary>
        /// Set the custom property to the container builder.
        /// It will be used as an argument for the custom container constructor.
        /// </summary>
        private static IContainerSkeleton SetProperty<T>(this IContainerSkeleton skeleton, T value)
        {
            if (skeleton.Properties.TryAdd(typeof(T), value))
                return skeleton;

            skeleton.Properties[typeof(T)] = value;
            return skeleton;
        }

        private static T GetProperty<T>(this IContainerSkeleton skeleton)
        {
            skeleton.Properties.TryGetValue(typeof(T), out var value);
            return value is T concreteValue ? concreteValue : default;
        }

        private sealed class ContainerBuilderSkeleton<T> : IContainerSkeleton
            where T : BaseContainer
        {
            public Type Type => typeof(T);

            public IDictionary<Type, object> Properties { get; } = new Dictionary<Type, object>
            {
                { typeof(string), null },
                { typeof(IContainer), null },
                { typeof(IContainerBuilder), null },
                { typeof(ITypeConstructionInfoProvider), null },
                // 
                { typeof(IActivatorFactoriesResolver), null },
                { typeof(ScriptingBackend), ScriptingBackend.Undefined },
                { typeof(BindingsCollectionFactory), null },
            };

            public object[] GetArguments(IReadOnlyList<Type> types)
            {
                Properties[typeof(IContainerBuilder)] =
                    this; //Make sure the container builder is available to the container

                var length = types.Count;
                var arguments = new object[length];
                for (var index = 0; index < length; index++)
                {
                    if (Properties.TryGetValue(types[index], out var value))
                        arguments[index] = value;
                }

                return arguments;
            }

            public IContainerSkeleton Clone()
            {
                var clone = new ContainerBuilderSkeleton<T>();
                clone.Copy(this);
                clone.Properties[typeof(string)] = null;
                clone.Properties[typeof(IContainer)] = null;
                return clone;
            }

            public void Copy(IContainerSkeleton source)
            {
                foreach (var (key, value) in source.Properties)
                {
                    if (Properties.TryAdd(key, value))
                        continue;

                    Properties[key] = value;
                }

                Properties[typeof(IContainerBuilder)] = this;
            }
        }
    }
}