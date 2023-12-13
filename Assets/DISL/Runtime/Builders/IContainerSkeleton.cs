using System;
using System.Collections.Generic;

namespace DISL.Runtime.Builders
{
    internal interface IContainerSkeleton : IContainerBuilder
    {
        Type Type { get; }
        public IDictionary<Type, object> Properties { get; }


        object[] GetArguments(IReadOnlyList<Type> types);
        IContainerSkeleton Clone();
        void Copy(IContainerSkeleton source);
    }
}