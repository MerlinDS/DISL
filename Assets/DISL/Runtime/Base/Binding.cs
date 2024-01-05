using System;
using DISL.Runtime.Resolvers;

namespace DISL.Runtime.Base
{
    internal sealed class Binding : IDisposable
    {
        public IResolver Resolver { get; }
        public Type[] Contracts { get; }

        private Binding(IResolver resolver, Type[] contracts)
        {
            Resolver = resolver;
            Contracts = contracts;
        }

        public static Binding Create<T>(IResolver resolver, params Type[] contracts) => 
            Create(resolver, typeof(T), contracts);

        public static Binding Create(IResolver resolver, Type concrete, params Type[] contracts)
        {
            foreach (var contract in contracts)
            {
                if (!contract.IsAssignableFrom(concrete))
                {
                    throw new ArgumentException(
                        $"Type {concrete.FullName} is not assignable from {contract.FullName}");
                }
            }
            
            return new Binding(resolver, contracts);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Resolver?.Dispose();
        }
    }
}