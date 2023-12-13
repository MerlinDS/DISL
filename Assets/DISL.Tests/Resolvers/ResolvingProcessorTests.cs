using System;
using System.Collections.Generic;
using DISL.Runtime.Bindings;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Reflections;
using DISL.Runtime.Resolvers;
using DISL.Tests.Utils;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Resolvers
{
    [TestFixture]
    public class ResolvingProcessorTests
    {
        [Test]
        public void Resolve_When_bind_with_instance_in_same_container_Should_return_instance()
        {
            //Arrange
            var expected = Substitute.For<TestConcreteImplementation>();
            var sut = CreateProcessor((bindings, instances, _) =>
            {
                bindings.ContainsKey(typeof(ITestInterface)).Returns(true);
                bindings[typeof(ITestInterface)].Returns(new BindingDescriptor(typeof(TestConcreteImplementation),
                    true));
                instances.ContainsKey(typeof(ITestInterface)).Returns(true);
                instances[typeof(ITestInterface)].Returns(expected);
            });
            //Act
            var actual = sut.Resolve(typeof(ITestInterface));
            //Assert
            actual.Should().Be(expected);
        }

        [Test]
        public void Resolve_When_bind_with_instance_in_parent_container_Should_return_instance()
        {
            //Arrange
            var expected = Substitute.For<TestConcreteImplementation>();
            var parent = CreateProcessor((bindings, instances, _) =>
            {
                bindings.ContainsKey(typeof(ITestInterface)).Returns(true);
                bindings[typeof(ITestInterface)].Returns(new BindingDescriptor(typeof(TestConcreteImplementation),
                    true));
                instances.ContainsKey(typeof(ITestInterface)).Returns(true);
                instances[typeof(ITestInterface)].Returns(expected);
            });

            var sut = CreateProcessor(parent);
            //Act
            var actual = sut.Resolve(typeof(ITestInterface));
            //Assert
            actual.Should().Be(expected);
        }

        [Test]
        public void Resolve_When_bind_to_single_Should_return_same_instance()
        {
            //Arrange
            var sut = CreateProcessor((bindings, _, t) =>
            {
                bindings.ContainsKey(typeof(ITestInterface)).Returns(true);
                bindings[typeof(ITestInterface)].Returns(new BindingDescriptor(typeof(TestConcreteImplementation),
                    true));

                t.Get(typeof(TestConcreteImplementation)).Returns(new TypeConstructionInfo(
                    _ => new TestConcreteImplementation(), Array.Empty<Type>()));
            });
            //Act
            var actual1 = sut.Resolve(typeof(ITestInterface));
            var actual2 = sut.Resolve(typeof(ITestInterface));
            //Assert
            actual1.Should().BeSameAs(actual2);
        }

        [Test]
        public void Resolve_When_bind_to_transient_Should_return_different_instance()
        {
            //Arrange
            var sut = CreateProcessor((bindings, _, t) =>
            {
                bindings.ContainsKey(typeof(ITestInterface)).Returns(true);
                bindings[typeof(ITestInterface)].Returns(new BindingDescriptor(typeof(TestConcreteImplementation)));

                t.Get(typeof(TestConcreteImplementation)).Returns(new TypeConstructionInfo(
                    _ => new TestConcreteImplementation(), Array.Empty<Type>()));
            });
            //Act
            var actual1 = sut.Resolve(typeof(ITestInterface));
            var actual2 = sut.Resolve(typeof(ITestInterface));
            //Assert
            actual1.Should().NotBeSameAs(actual2);
        }

        [Test]
        public void Resolve_When_bind_to_single_with_bind_parameters_Should_return_instance()
        {
            //Arrange
            var sut = CreateProcessor((bindings, _, t) =>
            {
                //Add ITestInterface
                bindings.ContainsKey(typeof(ITestInterface)).Returns(true);
                bindings[typeof(ITestInterface)].Returns(new BindingDescriptor(typeof(TestConcreteImplementation),
                    true));
                t.Get(typeof(TestConcreteImplementation)).Returns(new TypeConstructionInfo(
                    _ => new TestConcreteImplementation(), Array.Empty<Type>()));
                //Add TestClassWithNested

                bindings.ContainsKey(typeof(TestClassWithNested)).Returns(true);
                bindings[typeof(TestClassWithNested)].Returns(new BindingDescriptor(typeof(TestClassWithNested),
                    true));
                t.Get(typeof(TestClassWithNested)).Returns(new TypeConstructionInfo(
                    x => new TestClassWithNested((ITestInterface)x[0]),
                    new[] { typeof(ITestInterface) }));
            });
            //Act
            var actual = sut.Resolve(typeof(TestClassWithNested));
            //Assert
            actual.Should().BeOfType<TestClassWithNested>();
            actual.As<TestClassWithNested>().Nested.Should().BeOfType<TestConcreteImplementation>();
        }
        
        [Test]
        public void Resolve_When_bind_to_single_with_not_bind_parameters_Should_throw()
        {
            //Arrange
            var sut = CreateProcessor((bindings, _, t) =>
            {
                //Add TestClassWithNested

                bindings.ContainsKey(typeof(TestClassWithNested)).Returns(true);
                bindings[typeof(TestClassWithNested)].Returns(new BindingDescriptor(typeof(TestClassWithNested),
                    true));
                t.Get(typeof(TestClassWithNested)).Returns(new TypeConstructionInfo(
                    x => new TestClassWithNested((ITestInterface)x[0]),
                    new[] { typeof(ITestInterface) }));
            });
            Action act = () => sut.Resolve(typeof(TestClassWithNested));
            //Act & Assert
            act.Should().Throw<DISLInvalidConstructionException>().Which.InnerException.Should()
                .BeOfType<DISLBindingRegistrationException>();
        }

        private static ResolvingProcessor CreateProcessor(Factory factory = null) =>
            CreateProcessor(null, factory);

        private static ResolvingProcessor CreateProcessor(IResolvingProcessor parent,
            Factory factory = null)
        {
            var provider = Substitute.For<ITypeConstructionInfoProvider>();
            var bindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var instances = Substitute.For<IDictionary<Type, object>>();
            var processor = new ResolvingProcessor(parent, provider, bindings, instances);
            factory?.Invoke(bindings, instances, provider);
            return processor;
        }

        private delegate void Factory(IDictionary<Type, BindingDescriptor> bindings,
            IDictionary<Type, object> instances, ITypeConstructionInfoProvider provider);

        private class TestClassWithNested
        {
            public ITestInterface Nested { get; }

            public TestClassWithNested(ITestInterface nested)
            {
                Nested = nested;
            }
        }
    }
}