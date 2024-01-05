using System;
using System.Collections.Generic;
using System.Linq;
using DISL.Runtime.Base;
using DISL.Runtime.Generics;
using DISL.Runtime.Installers;
using DISL.Runtime.Reflections;
using DISL.Runtime.Resolvers;
using DISL.Runtime.Utils;

namespace DISL.Runtime.Builders
{
    public sealed class DISLBuilder
    {
        private readonly string _rootName;
        private readonly List<Binding> _installers = new(1);

        public static DISLBuilder Create(string rootName = null)
        {
            if (string.IsNullOrEmpty(rootName))
                rootName = "Root";
            return new DISLBuilder(rootName);
        }

        private DISLBuilder(string rootName) =>
            _rootName = rootName;

        public DISLBuilder WithInstaller<T>() where T : IContainerInstaller
        {
            AddInstallerBinding(typeof(T));
            return this;
        }

        public DISLBuilder WithBindingInstaller<T>() where T : IBindingInstaller
        {
            AddInstallerBinding(typeof(T));
            return this;
        }

        private void AddInstallerBinding(Type type) =>
            _installers.Add(Binding.Create(new SingletonTypeResolver(type), type));

        /// <summary>
        /// Build container
        /// </summary>
        /// <returns></returns>
        public IDisposable Build()
        {
            var scriptingBackend = ScriptingBackendDetector.Detect();
            var activatorFactory = ActivatorFactoriesResolver.Default().Resolve(scriptingBackend);
            var reflector = new Reflector(activatorFactory);

            var container = new Container(_rootName, reflector);
            var (bindingInstallers, containerInstallers) =
                ConstructInstallers(container);

            ExecuteBindingBuilders(bindingInstallers, container);
            var installers = ExecuteContainerInstallers(containerInstallers, container);

            var disposable = new DisposableCollection(installers);
            disposable.TryAdd(container);

            return disposable;
        }

        private (Queue<IBindingInstaller>, Queue<IContainerInstaller>) ConstructInstallers(IContainer container)
        {
            var bindingInstallers = new Queue<IBindingInstaller>();
            var containerInstallers = new Queue<IContainerInstaller>();
            foreach (var installer in _installers.Select(binding => binding.Resolver.Resolve(container)))
            {
                switch (installer)
                {
                    case IBindingInstaller bindingInstaller:
                        bindingInstallers.Enqueue(bindingInstaller);
                        break;
                    case IContainerInstaller containerInstaller:
                        containerInstallers.Enqueue(containerInstaller);
                        break;
                }
            }

            return (bindingInstallers, containerInstallers);
        }

        private static void ExecuteBindingBuilders(
            Queue<IBindingInstaller> bindingInstallers, IContainer container)
        {
            var containerBuilder = ContainerBuilderExtensions.Create(container.Name);
            while (bindingInstallers.TryDequeue(out var bindingInstaller))
            {
                bindingInstaller.Install(containerBuilder);
                if (bindingInstaller is IDisposable disposableInstaller)
                    disposableInstaller.Dispose();
            }

            containerBuilder.Build(container);
        }

        private static IEnumerable<IDisposable> ExecuteContainerInstallers(
            Queue<IContainerInstaller> containerInstallers, IContainer container)
        {
            while (containerInstallers.TryDequeue(out var containerInstaller))
            {
                containerInstaller.Install(container);
                if (containerInstaller is IDisposable disposableInstaller)
                    yield return disposableInstaller;
            }
        }
    }
}