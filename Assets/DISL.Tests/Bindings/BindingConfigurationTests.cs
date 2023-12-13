using System;
using DISL.Runtime.Bindings;
using DISL.Tests.Utils;
using NSubstitute;
using NUnit.Framework;

namespace DISL.Tests.Bindings
{
    [TestFixture]
    public class BindingConfigurationTests
    {
        [Test]
        public void AsSingle_When_binding_not_single_Should_set_single()
        {
            //Arrange
            var setSingle = Substitute.For<Action<Type>>();
            var sut = new BindingConfigure<ITestInterface, TestConcreteImplementation>(setSingle, 
                null!, null!);
            //Act
            sut.AsSingle();
            //Assert
            setSingle.Received(1).Invoke(Arg.Is<Type>(x => x == typeof(ITestInterface)));
        }

        [Test]
        public void NonLazy_When_binding_not_lazy_Should_set_non_lazy()
        {
            //Arrange
            var setNonLazy = Substitute.For<Action<Type>>();
            var sut = new BindingConfigure<ITestInterface, TestConcreteImplementation>(null!, 
                setNonLazy, null!);
            //Act
            sut.NonLazy();
            //Assert
            setNonLazy.Received(1).Invoke(Arg.Is<Type>(x => x == typeof(ITestInterface)));
        }

        [Test]
        public void ToInstance_Should_set_implementation_and_instance()
        {
            //Arrange
            var setInstance = Substitute.For<Action<Type, object>>();
            var sut = new BindingConfigure<ITestInterface, TestConcreteImplementation>(null!, 
                null!, setInstance);
            var instance = Substitute.For<TestConcreteImplementation>();
            //Act
            sut.ToInstance(instance);
            //Assert
            setInstance.Received(1).Invoke(Arg.Is<Type>(x => x == typeof(ITestInterface)), 
                Arg.Is<object>(x => x == instance));
        }
    }
}