using System;
using System.Reflection;
using DISL.Runtime.Base;
using DISL.Runtime.Builders;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Reflections;
using DISL.Runtime.Utils;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Builders
{
    [TestFixture]
    public class ContainerBuilderTests
    {
        [Test]
        public void Create_When_called_Should_return_new_default_builder()
        {
            //Act & Assert
            ContainerBuilder.Create().Should().NotBeNull();
        }

        [Test]
        public void Create_When_called_with_type_Should_return_new_builder()
        {
            //Act & Assert
            ContainerBuilder.Create<CustomContainer>().Should().NotBeNull();
        }

        [Test]
        public void GetSkeleton_When_called_with_invalid_builder_Should_throw_exception()
        {
            //Arrange
            var builder = Substitute.For<IContainerBuilder>();
            var act = new Action(() => builder.For(ScriptingBackend.Mono));
            //Act & Assert
            act.Should().Throw<DISLInvalidBuilderException>();
        }

        [Test]
        public void Build_When_called_Should_return_new_default_container()
        {
            //Act
            var actual = ContainerBuilder.Create().Build();
            //Assert
            actual.Should().NotBeNull();

            actual.Name.Should().BeNull();
            actual.Parent.Should().BeNull();

            actual.Should().BeOfType<Container>();
        }

        [Test]
        public void Build_When_custom_container_Should_return_new_custom_container()
        {
            //Arrange
            const int customProperty = 42;
            var builder = ContainerBuilder.Create<CustomContainer>()
                .WithProperty(customProperty);
            //Act
            var actual = builder.Build();
            //Assert
            actual.Should().NotBeNull();

            actual.Name.Should().BeNull();
            actual.Parent.Should().BeNull();

            actual.Should().BeOfType<CustomContainer>();
            actual.As<CustomContainer>().Collection.Should().BeOfType<BindingsCollection>();
            actual.As<CustomContainer>().TypeConstructionInfoProvider.Should()
                .BeOfType<TypeConstructionInfoProvider>();
            actual.As<CustomContainer>().CustomProperty.Should().Be(customProperty);
        }

        [Test]
        public void Build_When_custom_property_not_set_Should_throws_DISLInvalidConstructionException()
        {
            //Arrange
            var builder = ContainerBuilder.Create<CustomContainer>();
            //Act
            var act = new Action(() => builder.Build());
            //Assert
            act.Should().Throw<DISLInvalidConstructionException>();
        }

        [Test]
        public void Build_When_custom_container_with_specifications_Should_return_new_custom_container()
        {
            //Arrange
            const int customProperty = 42;
            const string name = "Test";
            const ScriptingBackend scriptingBackend = ScriptingBackend.Mono;
            var mockParent = Substitute.For<IContainer>();
            var mockCollection = Substitute.For<IBindingsCollection>();
            var mockCollectionFactory = Substitute.For<BindingsCollectionFactory>();
            mockCollectionFactory.Invoke().Returns(mockCollection);
            //Act
            var actual = ContainerBuilder.Create<CustomContainer>()
                .WithName(name)
                .WithParent(mockParent)
                .WithProperty(customProperty)
                .For(scriptingBackend)
                .With(mockCollectionFactory)
                .With(CreateMockActivatorFactoriesResolver(scriptingBackend))
                .Build();
            //Assert
            actual.Should().NotBeNull();

            actual.Name.Should().Be(name);
            actual.Parent.Should().Be(mockParent);

            actual.Should().BeOfType<CustomContainer>();
            actual.As<CustomContainer>().Collection.Should().Be(mockCollection);
            actual.As<CustomContainer>().TypeConstructionInfoProvider.Should()
                .BeOfType<TypeConstructionInfoProvider>();
            actual.As<CustomContainer>().CustomProperty.Should().Be(customProperty);
        }

        [Test]
        public void GetProvider_When_value_not_set_Should_throw_DISLException()
        {
            //Arrange
            var builder = ContainerBuilder.Create();
            //Act
            var act = new Action(() => builder.GetProvider());
            //Assert
            act.Should().Throw<DISLException>();
        }

        [Test]
        public void GetProvider_When_value_set_Should_return_value()
        {
            //Arrange
            var builder = ContainerBuilder.Create();
            builder.Build();
            //Act
            var actual = builder.GetProvider();
            //Assert
            actual.Should().NotBeNull();
            actual.Should().BeOfType<TypeConstructionInfoProvider>();
        }

        [Test]
        public void BuildCollection_When_value_not_set_Should_throw_DISLException()
        {
            //Arrange
            var builder = ContainerBuilder.Create();
            //Act
            var act = new Action(() => builder.BuildCollection());
            //Assert
            act.Should().Throw<DISLException>();
        }

        [Test]
        public void BuildCollection_When_value_set_Should_return_value()
        {
            //Arrange
            var builder = ContainerBuilder.Create();
            builder.Build();
            //Act
            var actual = builder.BuildCollection();
            //Assert
            actual.Should().NotBeNull();
            actual.Should().BeOfType<BindingsCollection>();
        }

        [Test]
        public void Clone_When_called_Should_return_new_builder()
        {
            //Arrange
            var builder = ContainerBuilder.Create<CustomContainer>();
            builder.WithName("Test")
                .WithParent(Substitute.For<IContainer>())
                .WithProperty(42);
            var parent = builder.Build();
            //Act
            var actual = builder.Clone();
            var actualContainer = actual.Build();
            //Assert
            actual.Should().NotBeNull();
            actual.Should().NotBeSameAs(builder);
            actualContainer.Should().NotBeNull();
            actualContainer.Should().NotBeSameAs(parent);
            actualContainer.Name.Should().BeNull();
            actualContainer.Parent.Should().BeNull();
            actualContainer.As<CustomContainer>().TypeConstructionInfoProvider.Should()
                .Be(parent.As<CustomContainer>().TypeConstructionInfoProvider);
        }


        private static IActivatorFactoriesResolver CreateMockActivatorFactoriesResolver(ScriptingBackend backend)
        {
            var activatorFactories = Substitute.For<IActivatorFactoriesResolver>();
            var activatorFactory = Substitute.For<IActivatorFactory>();
            activatorFactories.Resolve(backend).Returns(activatorFactory);

            activatorFactory.GenerateActivator(Arg.Any<Type>(), Arg.Any<ConstructorInfo>(),
                Arg.Any<Type[]>()).Returns(ReturnThis);
            return activatorFactories;
        }

        private static object ReturnThis(object[] x) =>
            new CustomContainer(x[0].As<string>(), x[1].As<IContainer>(), x[2].As<IContainerBuilder>(),
                x[3].As<int>());

        private sealed class CustomContainer : BaseContainer
        {
            public int CustomProperty { get; }
            public IBindingsCollection Collection { get; }
            public ITypeConstructionInfoProvider TypeConstructionInfoProvider { get; }

            /// <inheritdoc />
            public CustomContainer(string name, IContainer parent, IContainerBuilder builder, int customProperty) :
                base(name, parent, builder)
            {
                CustomProperty = customProperty;
                Collection = builder.BuildCollection();
                TypeConstructionInfoProvider = builder.GetProvider();
            }
        }
    }
}