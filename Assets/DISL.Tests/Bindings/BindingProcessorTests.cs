using System;
using System.Collections.Generic;
using DISL.Runtime.Bindings;
using DISL.Runtime.Exceptions;
using DISL.Tests.Utils;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Bindings
{
    [TestFixture]
    public class BindingProcessorTests
    {
        [Test]
        public void Register_When_type_not_registered_Should_register()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            var sut = new BindingProcessor(mockBindings, mockInstances);
            //Act
            sut.Register(typeof(ITestInterface));
            //Assert
            mockBindings.Received(1).Add(typeof(ITestInterface),
                Arg.Is<BindingDescriptor>(x => x.ImplementationType == typeof(ITestInterface)));
            mockInstances.Received(1).Add(typeof(ITestInterface), null);
        }

        [Test]
        public void Register_When_type_already_registered_Should_throw()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(true);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            Action act = () => sut.Register(typeof(ITestInterface));
            //Act & Assert
            act.Should().Throw<DISLBindingRegistrationException>().Which
                .Message.Should().Be("Binding for type DISL.Tests.Utils.ITestInterface already registered.");
        }

        [Test]
        public void SetImplementation_When_type_registered_and_implementation_type_is_valid_Should_set_implementation()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(true);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            //Act
            sut.SetImplementation(typeof(ITestInterface), typeof(TestConcreteImplementation));
            //Assert
            mockBindings.Received(1)[Arg.Is<Type>(x => x == typeof(ITestInterface))]
                = Arg.Is<BindingDescriptor>(x => x.ImplementationType == typeof(TestConcreteImplementation));
            mockInstances.Received(1)[Arg.Is<Type>(x => x == typeof(ITestInterface))] = Arg.Is<object>(x => x == null);
        }

        [Test]
        public void SetImplementation_When_type_not_registered_Should_throw()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(false);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            Action act = () => sut.SetImplementation(typeof(ITestInterface), typeof(TestConcreteImplementation));
            //Act & Assert
            act.Should().Throw<DISLBindingRegistrationException>().Which
                .Message.Should().Be("Binding for type DISL.Tests.Utils.ITestInterface not registered.");
        }

        [Test]
        public void SetImplementation_When_type_registered_and_implementation_type_is_abstract_Should_throw()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(true);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            Action act = () => sut.SetImplementation(typeof(ITestInterface), typeof(TestAbstractImplementation));
            //Act & Assert
            act.Should().Throw<DISLBindingImplementationException>().Which
                .Message.Should()
                .Be(
                    "Type DISL.Tests.Utils.TestAbstractImplementation is abstract and can't be used as implementation.");
        }

        [Test]
        public void SetImplementation_When_type_registered_and_implementation_type_is_not_assignable_Should_throw()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(true);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            Action act = () => sut.SetImplementation(typeof(ITestInterface), typeof(string));
            //Act & Assert
            act.Should().Throw<DISLBindingImplementationException>().Which
                .Message.Should().Be("Type System.String is not assignable from DISL.Tests.Utils.ITestInterface.");
        }

        [Test]
        public void SetSingle_When_type_is_registered_Should_set_single()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(true);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            //Act
            sut.SetSingle(typeof(ITestInterface));
            //Assert
            mockBindings.Received(1)[Arg.Is<Type>(x => x == typeof(ITestInterface))]
                = Arg.Is<BindingDescriptor>(x => x.IsSingle);
        }

        [Test]
        public void SetSingle_When_type_is_not_registered_Should_throw()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(false);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            Action act = () => sut.SetSingle(typeof(ITestInterface));
            //Act & Assert
            act.Should().Throw<DISLBindingRegistrationException>().Which
                .Message.Should().Be("Binding for type DISL.Tests.Utils.ITestInterface not registered.");
        }

        [Test]
        public void SetNonLazy_When_type_is_registered_Should_set_non_lazy()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(true);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            //Act
            sut.SetNonLazy(typeof(ITestInterface));
            //Assert
            mockBindings.Received(1)[Arg.Is<Type>(x => x == typeof(ITestInterface))]
                = Arg.Is<BindingDescriptor>(x => x.IsNonLazy);
        }

        [Test]
        public void SetNonLazy_When_type_is_not_registered_Should_throw()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(false);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            Action act = () => sut.SetNonLazy(typeof(ITestInterface));
            //Act & Assert
            act.Should().Throw<DISLBindingRegistrationException>().Which
                .Message.Should().Be("Binding for type DISL.Tests.Utils.ITestInterface not registered.");
        }

        [Test]
        public void
            SetInstance_When_type_is_registered_Should_set_instance_and_destroy_previous_instance_if_disposable()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();
            var mockInstance = Substitute.For<IDisposable>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(true);
            mockInstances[typeof(ITestInterface)].Returns(mockInstance);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            //Act
            sut.SetInstance(typeof(ITestInterface), mockInstance);
            //Assert
            mockInstances.Received(1)[Arg.Is<Type>(x => x == typeof(ITestInterface))] =
                Arg.Is<object>(x => x == mockInstance);
            mockInstance.Received(1).Dispose();
        }

        [Test]
        public void SetInstance_When_type_is_not_registered_Should_throw()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();
            var mockInstance = Substitute.For<IDisposable>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(false);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            Action act = () => sut.SetInstance(typeof(ITestInterface), mockInstance);
            //Act & Assert
            act.Should().Throw<DISLBindingRegistrationException>().Which
                .Message.Should().Be("Binding for type DISL.Tests.Utils.ITestInterface not registered.");
        }
        
        [Test]
        public void Unregister_When_type_is_registered_Should_unregister()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();
            var mockInstance = Substitute.For<IDisposable>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(true);
            mockInstances[typeof(ITestInterface)].Returns(mockInstance);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            //Act
            sut.Unregister(typeof(ITestInterface));
            //Assert
            mockBindings.Received(1).Remove(typeof(ITestInterface));
            mockInstances.Received(1).Remove(typeof(ITestInterface));
            mockInstance.Received(1).Dispose();
        }
        
        [Test]
        public void Unregister_When_type_is_not_registered_Should_throw()
        {
            //Arrange
            var mockBindings = Substitute.For<IDictionary<Type, BindingDescriptor>>();
            var mockInstances = Substitute.For<IDictionary<Type, object>>();

            mockBindings.ContainsKey(typeof(ITestInterface)).Returns(false);

            var sut = new BindingProcessor(mockBindings, mockInstances);
            Action act = () => sut.Unregister(typeof(ITestInterface));
            //Act & Assert
            act.Should().Throw<DISLBindingRegistrationException>().Which
                .Message.Should().Be("Binding for type DISL.Tests.Utils.ITestInterface not registered.");
        }
    }
}