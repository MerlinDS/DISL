using System;
using System.Reflection;
using DISL.Runtime.Builders;
using DISL.Runtime.Delegates;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Reflections;
using DISL.Runtime.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace DISL.Tests.Builders
{
    [TestFixture]
    public class ActivatorFactoriesResolverTests
    {
        [Test]
        public void Resolve_When_scripting_backend_added_Should_return_activator_factory()
        {
            //Arrange
            var sut = new ActivatorFactoriesResolver();
            sut.Add<TestFactory>(ScriptingBackend.Mono);
            //Act
            var actual = sut.Resolve(ScriptingBackend.Mono);
            //Assert
            actual.Should().BeOfType<TestFactory>();
        }

        [Test]
        public void Resolve_When_scripting_backend_added_twice_Should_return_other_activator_factory()
        {
            //Arrange
            var sut = new ActivatorFactoriesResolver();
            sut.Add<TestFactory>(ScriptingBackend.Mono);
            var unexpected = sut.Resolve(ScriptingBackend.Mono);
            sut.Add<TestFactory>(ScriptingBackend.Mono);
            //Act
            var actual = sut.Resolve(ScriptingBackend.Mono);
            //Assert
            actual.Should().BeOfType<TestFactory>();
            actual.Should().NotBe(unexpected);
        }

        [Test]
        public void Resolve_When_scripting_backend_added_and_cached_Should_return_cached_activator_factory()
        {
            //Arrange
            var sut = new ActivatorFactoriesResolver();
            sut.Add<TestFactory>(ScriptingBackend.Mono);
            var expected = sut.Resolve(ScriptingBackend.Mono);
            //Act
            var actual = sut.Resolve(ScriptingBackend.Mono);
            //Assert
            actual.Should().BeOfType<TestFactory>();
            actual.Should().Be(expected);
        }

        [Test]
        public void Resolve_When_scripting_backend_not_added_Should_throw_exception()
        {
            //Arrange
            var sut = new ActivatorFactoriesResolver();
            //Act
            Action action = () => sut.Resolve(ScriptingBackend.Mono);
            //Assert
            action.Should().Throw<DISLRuntimeScriptingBackendException>();
        }

        [Test]
        public void Default_When_called_Should_return_default_resolver()
        {
            //Arrange
            var sut = ActivatorFactoriesResolver.Default();
            //Act & Assert
            sut.Resolve(ScriptingBackend.Mono).Should().BeOfType<MonoActivatorFactory>();
            sut.Resolve(ScriptingBackend.IL2CPP).Should().BeOfType<IL2CPPActivatorFactory>();
            var act = new Action(() => sut.Resolve(ScriptingBackend.Undefined));
            act.Should().Throw<DISLRuntimeScriptingBackendException>();
        }

        private sealed class TestFactory : IActivatorFactory
        {
            /// <inheritdoc />
            public ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public ObjectActivator GenerateDefaultActivator(Type type)
            {
                throw new NotImplementedException();
            }
        }
    }
}