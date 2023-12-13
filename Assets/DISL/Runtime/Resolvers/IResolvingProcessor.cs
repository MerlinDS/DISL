using System;

namespace DISL.Runtime.Resolvers
{
    internal interface IResolvingProcessor
    {
        object Resolve(Type type);
    }
}