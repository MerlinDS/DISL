using System;
using System.Reflection;
using DISL.Runtime.Reflections;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Reflections
{
    [TestFixture]
    public class TypeConstructionInfoProviderTests
    {
        [Test]
        public void Get_When_no_parameters_Should_return_TypeConstructionInfo()
        {
            //Arrange
            var mockActivatorFactory = Substitute.For<IActivatorFactory>();
            mockActivatorFactory.GenerateActivator(typeof(TestParameterlessClass),
                    Arg.Any<ConstructorInfo>(), Arg.Any<Type[]>())
                .Returns(_ => new TestParameterlessClass());

            var sut = new TypeConstructionInfoProvider(mockActivatorFactory);
            //Act
            var actual = sut.Get(typeof(TestParameterlessClass));
            //Assert
            actual.Should().NotBeNull();
            actual.Activator.Should().NotBeNull();
            actual.Parameters.Should().BeEmpty();
            var invoke = actual.Activator.Invoke(Array.Empty<object>());
            invoke.Should().BeOfType<TestParameterlessClass>();
        }

        [Test]
        public void Get_When_has_parameters_Should_return_TypeConstructionInfo()
        {
            //Arrange
            var mockActivatorFactory = Substitute.For<IActivatorFactory>();
            mockActivatorFactory.GenerateActivator(typeof(TestClass), Arg.Any<ConstructorInfo>(),
                    Arg.Any<Type[]>())
                .Returns(_ => new TestClass(0, string.Empty));

            var sut = new TypeConstructionInfoProvider(mockActivatorFactory);
            //Act
            var actual = sut.Get(typeof(TestClass));
            //Assert
            actual.Should().NotBeNull();
            actual.Activator.Should().NotBeNull();
            actual.Parameters.Should().NotBeEmpty();
            actual.Activator.Invoke(0, string.Empty).Should().BeOfType<TestClass>();
        }

        private class TestParameterlessClass
        {
        }

        private class TestClass
        {
            // ReSharper disable UnusedParameter.Local
            public TestClass(int a, string b)
                // ReSharper restore UnusedParameter.Local
            {
            }
        }
    }
}