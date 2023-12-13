using System;
using DISL.Runtime.Bindings;
using DISL.Tests.Utils;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace DISL.Tests.Bindings
{
    [TestFixture]
    public class BindingTests
    {
        [Test]
        public void Ctor_When_processor_is_null_Should_throw()
        {
            //Arrange
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new Binding<ITestInterface>(null!);
            //Act & Assert
            act.Should().Throw<ArgumentNullException>();
        }
        
        [Test]
        public void To_Should_SetImplementation()
        {
            //Arrange
            var mockProcessor = Substitute.For<IBindingProcessor>();
            var sut = new Binding<ITestInterface>(mockProcessor);
            //Act
            sut.To<TestConcreteImplementation>().AsSingle();
            //Assert
            mockProcessor.Received(1).SetImplementation(typeof(ITestInterface), typeof(TestConcreteImplementation));
            mockProcessor.Received(1).SetSingle(Arg.Any<Type>());
        }
    }
}