using System;

namespace DISL.Runtime.Base
{
    public interface IConstructor
    {
        internal object Construct(Type type, Func<Type, object> resolver = null);
    }
}