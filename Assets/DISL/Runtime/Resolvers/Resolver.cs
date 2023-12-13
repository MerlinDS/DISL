using System;

namespace DISL.Runtime.Resolvers
{
    public readonly ref struct Resolver<TType>
    {
        private readonly IResolvingProcessor _processor;

        private Resolver(IResolvingProcessor processor)
        {
            _processor = processor;
        }

        public TType Resolve()
        {
            var instance = _processor.Resolve(typeof(TType));
            return instance is TType type ? type : default;
        }
    }
}