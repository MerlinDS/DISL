using System;
using DISL.Runtime.Attributes;
using DISL.Runtime.Base;
using DISL.Runtime.Bindings;
using DISL.Runtime.Builders;
using DISL.Runtime.Installers;
using DISL.Tests.Utils;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Builders
{
    [TestFixture]
    public class DISLIntegrationTests
    {
        [Test]
        public void Test()
        {
            //Arrange
            CustomBinder.Instance = null;
            CustomContainer.Instance = null;
            CustomInstaller.Instance = null;
            var concreteCustomInstaller = Substitute.For<ConcreteCustomInstaller>();
            var concreteCustomBinder = Substitute.For<IBinder, IDisposable>();
            var builder = DISLBuilder.Create();
            //Act
            var disposable = builder
                .ConfigureRoot<CustomContainer>(x => x
                    .WithName("Root")
                    .WithProperty(42))
                .AddBinder<CustomBinder>()
                .AddBinder(concreteCustomBinder)
                .AddInstaller<CustomInstaller>()
                .AddInstaller(concreteCustomInstaller)
                .Build();
            //Assert
            disposable.Should().NotBeNull();
            //Assert root container
            CustomContainer.Instance.Should().NotBeNull();
            CustomContainer.Instance!.Property.Should().Be(42);
            //Assert binding
            CustomBinder.Instance.Should().BeNull();
            concreteCustomBinder.Received(1).Bind(Arg.Any<IBindingContainer>());
            concreteCustomBinder.As<IDisposable>().Received().Dispose();
            //Assert installation
            CustomInstaller.Instance.Should().NotBeNull();
            CustomInstaller.Instance!.TestConcreteImplementation.Should().NotBeNull();
            CustomInstaller.Instance.TestConcreteImplementation.Should().BeOfType<TestConcreteImplementation>();
            concreteCustomInstaller.Received(1).Install(Arg.Any<Container>());
            disposable.Dispose();
            //Assert disposing
            CustomContainer.Instance.Should().BeNull();
            CustomBinder.Instance.Should().BeNull();
            CustomInstaller.Instance.Should().BeNull();
        }

        private sealed class CustomContainer : BaseContainer
        {
            public int Property { get; private set; }
            public static CustomContainer Instance { get; set; }

            /// <inheritdoc />
            public CustomContainer(int property, string name, IContainer parent, IContainerBuilder builder) : base(name,
                parent,
                builder)
            {
                Property = property;
                Instance = this;
            }

            /// <inheritdoc />
            public override Binding<T> Bind<T>()
            {
                var bindingProcessor = Substitute.For<IBindingProcessor>();
                return new Binding<T>(bindingProcessor);
            }

            /// <inheritdoc />
            public override T Resolve<T>()
            {
                return base.Resolve<T>();
            }

            /// <inheritdoc />
            public override void Dispose()
            {
                Instance = null;
                base.Dispose();
            }
        }

        private sealed class CustomInstaller : IInstaller, IDisposable
        {
            public ITestInterface TestConcreteImplementation { get; private set; }

            public static CustomInstaller Instance { get; set; }

            public CustomInstaller()
            {
                Instance = this;
            }

            /// <inheritdoc />
            public void Install(IResolvingContainer container)
            {
                container.Should().NotBeNull();
                container.Should().BeOfType<CustomContainer>();
                // _testInterface = container.Resolve<ITestInterface>(); //TODO: Implement
            }

            /// <inheritdoc />
            public void Dispose()
            {
                Instance = null;
            }
        }

        internal abstract class ConcreteCustomInstaller : IInstaller
        {

            /// <inheritdoc />
            [Container("Child")]
            public abstract void Install(IResolvingContainer container);
        }

        private sealed class CustomBinder : IContainerConfigurator, IBinder, IDisposable
        {
            public static CustomBinder Instance { get; set; }

            public CustomBinder()
            {
                Instance = this;
            }

            /// <inheritdoc />
            public IContainerBuilder Configure(IContainerBuilder builder) =>
                ContainerBuilder.Inherit(builder, "Child");

            /// <inheritdoc />
            public void Bind(IBindingContainer container)
            {
                container.Should().NotBeNull();
                container.Should().BeOfType<Container>();
                // container.Bind<ITestInterface>().To<TestConcreteImplementation>();
            }

            /// <inheritdoc />
            public void Dispose()
            {
                Instance = null;
            }
        }
    }
}