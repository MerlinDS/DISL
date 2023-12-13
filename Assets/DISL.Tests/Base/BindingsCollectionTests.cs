using System;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Bindings;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Base
{
    [TestFixture]
    public class BindingsCollectionTests
    {
        private readonly IDictionary<Type, BindingDescriptor> _mockDescriptors =
            Substitute.For<IDictionary<Type, BindingDescriptor>>();

        private readonly IDictionary<Type, object> _mockInstances =
            Substitute.For<IDictionary<Type, object>>();


        [Test]
        public void GetInstance_When_has_type_Should_return_instance()
        {
            //Arrange
            var expected = new object();
            _mockDescriptors.ContainsKey(typeof(object)).Returns(true);
            _mockInstances[typeof(object)].Returns(expected);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            var actual = sut.GetInstance(typeof(object));
            //Assert 
            actual.Should().Be(expected);
        }

        [Test]
        public void GetInstance_When_has_no_type_Should_throw_KeyNotFoundException()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(false);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            Action action = () => sut.GetInstance(typeof(object));
            //Assert
            action.Should().Throw<KeyNotFoundException>();
        }

        [Test]
        public void SetInstance_When_has_type_Should_set_instance()
        {
            //Arrange
            var expected = new object();
            _mockDescriptors.ContainsKey(typeof(object)).Returns(true);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            sut.SetInstance(typeof(object), expected);
            //Assert
            _mockInstances.Received(1)[typeof(object)] = expected;
        }

        [Test]
        public void SetInstance_When_has_no_type_Should_throw_KeyNotFoundException()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(false);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            Action action = () => sut.SetInstance(typeof(object), new object());
            //Assert
            action.Should().Throw<KeyNotFoundException>();
        }
        
        [Test]
        public void Add_When_has_type_Should_throw_ArgumentException()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(true);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            Action action = () => sut.Add(typeof(object), new BindingDescriptor(typeof(object)));
            //Assert
            action.Should().Throw<ArgumentException>();
        }
        
        [Test]
        public void Add_When_has_no_type_Should_add_type()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(false);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            sut.Add(typeof(object), new BindingDescriptor(typeof(object)));
            //Assert
            _mockDescriptors.Received(1).Add(typeof(object), new BindingDescriptor(typeof(object)));
            _mockInstances.Received(1).Add(typeof(object), null);
        }
        
        [Test]
        public void Remove_When_has_type_Should_remove_type()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(true);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            sut.Remove(typeof(object));
            //Assert
            _mockDescriptors.Received(1).Remove(typeof(object));
            _mockInstances.Received(1).Remove(typeof(object));
        }
        
        [Test]
        public void Remove_When_has_no_type_Should_throw_KeyNotFoundException()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(false);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            Action action = () => sut.Remove(typeof(object));
            //Assert
            action.Should().Throw<KeyNotFoundException>();
        }
        
        [Test]
        public void Has_When_has_type_Should_return_true()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(true);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            var actual = sut.Has(typeof(object));
            //Assert
            actual.Should().BeTrue();
        }
        
        [Test]
        public void Has_When_has_no_type_Should_return_false()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(false);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            var actual = sut.Has(typeof(object));
            //Assert
            actual.Should().BeFalse();
        }
        
        [Test]
        public void SetDescriptor_When_has_type_Should_set_descriptor()
        {
            //Arrange
            var expected = new BindingDescriptor(typeof(object));
            _mockDescriptors.ContainsKey(typeof(object)).Returns(true);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            sut.SetDescriptor(typeof(object), expected);
            //Assert
            _mockDescriptors.Received(1)[typeof(object)] = expected;
        }
        
        [Test]
        public void SetDescriptor_When_has_no_type_Should_throw_KeyNotFoundException()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(false);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            Action action = () => sut.SetDescriptor(typeof(object), new BindingDescriptor(typeof(object)));
            //Assert
            action.Should().Throw<KeyNotFoundException>();
        }
        
        [Test]
        public void GetDescriptor_When_has_type_Should_return_descriptor()
        {
            //Arrange
            var expected = new BindingDescriptor(typeof(object));
            _mockDescriptors.ContainsKey(typeof(object)).Returns(true);
            _mockDescriptors[typeof(object)].Returns(expected);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            var actual = sut.GetDescriptor(typeof(object));
            //Assert
            actual.Should().Be(expected);
        }
        
        [Test]
        public void GetDescriptor_When_has_no_type_Should_throw_KeyNotFoundException()
        {
            //Arrange
            _mockDescriptors.ContainsKey(typeof(object)).Returns(false);
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            Action action = () => sut.GetDescriptor(typeof(object));
            //Assert
            action.Should().Throw<KeyNotFoundException>();
        }
        
        [Test]
        public void Dispose_When_called_Should_clear_collections()
        {
            //Arrange
            var sut = new BindingsCollection(_mockDescriptors, _mockInstances);
            //Act
            sut.Dispose();
            //Assert
            _mockDescriptors.Received(1).Clear();
            _mockInstances.Received(1).Clear();
        }
    }
}