using System;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Bindings;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Installers;

namespace DISL.Runtime.Builders
{
    public static partial class DISLBuilder
    {
        /// <summary>
        /// Build the DISL containers, installers and binders.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IDisposable Build(this IDISLBuilder builder)
        {
            var skeleton = Prepare(builder);
            var root = BuildRootContainer(skeleton);
            SortInstances(skeleton);
            Configure(skeleton, root);
            Bind(skeleton, root);
            NonLazyResolving(skeleton);
            Install(skeleton, root);
            //TODO: Implement non lazy resolving
            return DisposeSkeleton(skeleton);
        }

        private static IDISLBuilderSkeleton Prepare(IDISLBuilder builder)
        {
            var skeleton = builder.GetSkeleton();
            skeleton.RootContainerBuilder ??= ContainerBuilder.Create();
            ValidateSkeleton(skeleton);
            return skeleton;
        }
        
        private static void ValidateSkeleton(IDISLBuilderSkeleton skeleton)
        {
            if (skeleton.BuildingTypes.Count == 0 && skeleton.Instances.Count == 0)
                throw new DISLException("No installers or binders were added to the builder.");
        }
        
        private static IContainer BuildRootContainer(IDISLBuilderSkeleton skeleton)
        {
            var root = skeleton.RootContainerBuilder.Build();
            skeleton.Disposables.Add(root);
            return root;
        }
        
        private static void SortInstances(IDISLBuilderSkeleton skeleton)
        {
            foreach (var instance in skeleton)
                switch (instance)
                {
                    case IInstaller installer:
                        skeleton.Installers.Enqueue(installer);
                        break;
                    case IBinder binder:
                        skeleton.Binders.Enqueue(binder);
                        if (instance is IContainerConfigurator configurator)
                            skeleton.Configurators.Enqueue(configurator);
                        break;
                }
        }

        private static void Configure(IDISLBuilderSkeleton skeleton, IContainer root)
        {
            foreach (var configurator in skeleton.Configurators)
            {
                var child = BuildChildContainer(skeleton, root, configurator);
                skeleton.ChildContainers.TryAdd(configurator.GetType(), child);
                skeleton.Disposables.Add(child);
            }
        }
        
        private static IContainer BuildChildContainer(IDISLBuilderSkeleton skeleton, IContainer root, IContainerConfigurator configurator)
        {
            var builder = skeleton.RootContainerBuilder.Clone().WithParent(root);
            return configurator.Configure(builder).Build();
        }

        private static void Bind(IDISLBuilderSkeleton skeleton, IContainer root)
        {
            foreach (var binder in skeleton.Binders)
            {
                var container = GetContainer(skeleton, root, binder.GetType());
                binder.Bind(container);
                if (binder is IDisposable disposable)
                    disposable.Dispose();
            }
        }
        
        private static void NonLazyResolving(IDISLBuilderSkeleton skeleton)
        {
            //TODO: Implement non lazy resolving
        }
        
        private static void Install(IDISLBuilderSkeleton skeleton, IContainer root)
        {
            foreach (var installer in skeleton.Installers)
            {
                
                var container = GetContainer(skeleton, root, installer.GetType());
                installer.Install(container);
                if (installer is IDisposable disposable)
                    skeleton.Disposables.Add(disposable);
            }
        }
        
        private static IContainer GetContainer(IDISLBuilderSkeleton skeleton, IContainer root, Type type)
        {
            //TODO: Get desires from type (if any) get from attributes
            return skeleton.ChildContainers.TryGetValue(type, out var container) ? container : root;
        }

        private static IDisposable DisposeSkeleton(IDISLBuilderSkeleton skeleton)
        {
            var disposables = skeleton.Disposables;
            skeleton.Dispose();
            return disposables;
        }
    }
}