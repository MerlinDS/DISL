using System;
using System.Collections.Generic;
using DISL.Runtime.Exceptions;
using DISL.Runtime.Reflections;
using DISL.Runtime.Utils;

namespace DISL.Runtime.Builders
{
    internal sealed class ActivatorFactoriesResolver
    {
        private readonly Dictionary<ScriptingBackend, Type> _activatorFactories = new();
        private readonly Dictionary<ScriptingBackend, IActivatorFactory> _cachedActivatorFactories = new();

        public static ActivatorFactoriesResolver Default()
        {
            var resolver = new ActivatorFactoriesResolver();
            resolver.Add<IL2CPPActivatorFactory>(ScriptingBackend.IL2CPP);
            resolver.Add<MonoActivatorFactory>(ScriptingBackend.Mono);
            return resolver;
        }

        public void Add<T>(ScriptingBackend scriptingBackend)
            where T : IActivatorFactory, new()
        {
            _activatorFactories.TryAdd(scriptingBackend, typeof(T));
            if (_cachedActivatorFactories.ContainsKey(scriptingBackend))
                _cachedActivatorFactories.Remove(scriptingBackend);
        }

        public IActivatorFactory Resolve(ScriptingBackend scriptingBackend)
        {
            if (!_activatorFactories.ContainsKey(scriptingBackend))
                throw new DISLRuntimeScriptingBackendException(scriptingBackend);

            if (!_cachedActivatorFactories.ContainsKey(scriptingBackend))
                _cachedActivatorFactories.Add(scriptingBackend, CreateActivatorFactory(scriptingBackend));

            return _cachedActivatorFactories[scriptingBackend];
        }

        private IActivatorFactory CreateActivatorFactory(ScriptingBackend scriptingBackend)
        {
            var type = _activatorFactories[scriptingBackend];
            return (IActivatorFactory)Activator.CreateInstance(type);//Could not be other type than IActivatorFactory
        }
    }
}