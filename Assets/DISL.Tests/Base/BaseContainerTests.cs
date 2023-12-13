using DISL.Runtime.Base;
using DISL.Runtime.Builders;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Base
{
    [TestFixture]
    public class BaseContainerTests
    {
        [Test]
        public void Ctor()
        {
            //Arrange
            const string expectedName = "Test";
            var expectedParent = Substitute.For<IContainer>();
            var expectedBuilder = Substitute.For<IContainerBuilder>();
            //Act
            var sut = new TestContainer(expectedName, expectedParent, expectedBuilder);
            //Assert
            sut.Name.Should().Be(expectedName);
            sut.Parent.Should().Be(expectedParent);
            sut.Builder.Should().Be(expectedBuilder);
        }

        private sealed class TestContainer : BaseContainer
        {
            public new IContainerBuilder Builder => base.Builder;

            /// <inheritdoc />
            public TestContainer(string name, IContainer parent, IContainerBuilder builder) : base(name, parent,
                builder)
            {
            }
        }
    }
}