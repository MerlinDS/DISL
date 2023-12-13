using System;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Builders;
using DISL.Runtime.Reflections;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Base
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void Ctor()
        {
            //Arrange
            const string expectedName = "Test";
            var mockParent = Substitute.For<IContainer>();
            var mockBuilder = Substitute.For<IContainerSkeleton>();
            var mockProperties = Substitute.For<IDictionary<Type, object>>();
            mockBuilder.Properties.Returns(mockProperties);
            mockBuilder.Type.Returns(typeof(Container));
            
            var mockBindingsCollectionFactory = Substitute.For<BindingsCollectionFactory>();
            var mockTypeConstructionInfoProvider = Substitute.For<ITypeConstructionInfoProvider>();
            
            mockProperties.TryGetValue(typeof(BindingsCollectionFactory), out Arg.Any<object>()).Returns(x =>
            {
                x[1] = mockBindingsCollectionFactory;
                return true;
            });
            
            mockProperties.TryGetValue(typeof(ITypeConstructionInfoProvider), out Arg.Any<object>()).Returns(x =>
            {
                x[1] = mockTypeConstructionInfoProvider;
                return true;
            });
            
            //Act
            var sut = new Container(expectedName, mockParent, mockBuilder);
            //Assert
            sut.Name.Should().Be(expectedName);
            sut.Parent.Should().Be(mockParent);
            mockBindingsCollectionFactory.Received().Invoke();
            mockProperties.Received().TryGetValue(typeof(ITypeConstructionInfoProvider), out Arg.Any<object>());
        }
    }
}