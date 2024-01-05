using System;
using DISL.Runtime.Base;

namespace DISL.Runtime.Resolvers
{
    public interface IResolver : IDisposable
    {
        object Resolve(IContainer container);
    }
}