using System;
using System.Collections.Generic;
using DISL.Runtime.Base;
using DISL.Runtime.Bindings;
using DISL.Runtime.Installers;

namespace DISL.Runtime.Builders
{
    internal interface IDISLBuilderSkeleton : IDISLBuilder, IEnumerable<object>, IDisposable
    {
        IContainerBuilder RootContainerBuilder { get; set; }
        IList<object> Instances { get; }
        IList<Type> BuildingTypes { get; }
        
        Queue<IBinder> Binders { get; }
        Queue<IInstaller> Installers { get; }
        Queue<IContainerConfigurator> Configurators { get; }
        IDictionary<Type, IContainer> ChildContainers { get; }
        DISLDisposables Disposables { get; }
    }
}