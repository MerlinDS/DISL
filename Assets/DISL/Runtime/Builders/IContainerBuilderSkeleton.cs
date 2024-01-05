using System.Collections.Generic;
using DISL.Runtime.Base;

namespace DISL.Runtime.Builders
{
    internal interface IContainerBuilderSkeleton : IContainerBuilder
    {
        string Name { get; set; }
        List<IContainerBuilderSkeleton> Children { get; }
        List<Binding> Bindings { get; }
    }
}