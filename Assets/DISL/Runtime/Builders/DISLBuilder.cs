using System;
using System.Collections;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Bindings;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Installers;

namespace DISL.Runtime.Builders
{
    public static partial class DISLBuilder
    {
        public static IDISLBuilder Create() =>
            new BuilderSkeleton();

        private static IDISLBuilderSkeleton GetSkeleton(this IDISLBuilder builder)
        {
            if (builder is not IDISLBuilderSkeleton skeleton)
                throw new DISLInvalidBuilderException(builder);
            return skeleton;
        }

        private sealed class BuilderSkeleton : IDISLBuilderSkeleton
        {
            /// <inheritdoc />
            public IContainerBuilder RootContainerBuilder { get; set; }

            /// <inheritdoc />
            public IList<object> Instances { get; } = new List<object>();

            /// <inheritdoc />
            public IList<Type> BuildingTypes { get; } = new List<Type>();

            /// <inheritdoc />
            public Queue<IBinder> Binders { get; } = new();

            /// <inheritdoc />
            public Queue<IInstaller> Installers { get; } = new();

            /// <inheritdoc />
            public Queue<IContainerConfigurator> Configurators { get; } = new();

            /// <inheritdoc />
            public IDictionary<Type, IContainer> ChildContainers { get; } = new Dictionary<Type, IContainer>();
            
            /// <inheritdoc />
            public DISLDisposables Disposables { get; private set; } = new();


            /// <inheritdoc />
            public void Dispose()
            {
                Disposables = null;
                RootContainerBuilder = null;
                
                Instances.Clear();
                BuildingTypes.Clear();
                Binders.Clear();
                Installers.Clear();
                Configurators.Clear();
                ChildContainers.Clear();
            }

            /// <inheritdoc />
            public IEnumerator<object> GetEnumerator()
            {
                foreach (var type in BuildingTypes)
                    yield return Activator.CreateInstance(type); //TODO: Use ITypeConstructionInfoProvider

                foreach (var instance in Instances)
                    yield return instance;
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}