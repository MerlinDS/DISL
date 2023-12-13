using System;
using DISL.Runtime.Base;
using DISL.Runtime.Bindings;
using DISL.Runtime.Installers;

namespace DISL.Runtime.Builders
{
    public static partial class DISLBuilder
    {
        public static IDISLBuilder ConfigureRoot(this IDISLBuilder builder,
            Func<IContainerBuilder, IContainerBuilder> setup)
        {
            return builder.ConfigureRoot<Container>(setup);
        }

        public static IDISLBuilder ConfigureRoot<T>(this IDISLBuilder builder,
            Func<IContainerBuilder, IContainerBuilder> setup)
            where T : BaseContainer
        {
            var skeleton = builder.GetSkeleton();
            if (setup is not null)
                skeleton.RootContainerBuilder = setup.Invoke(ContainerBuilder.Create<T>());
            return builder;
        }

        public static IDISLBuilder AddInstaller<T>(this IDISLBuilder builder, T installer) where T : IInstaller
        {
            return builder.GetSkeleton().AddInstance(installer);
        }

        public static IDISLBuilder AddInstaller<T>(this IDISLBuilder builder) where T : IInstaller
        {
            return builder.GetSkeleton().AddBuildingType<T>();
        }

        public static IDISLBuilder AddBinder<T>(this IDISLBuilder builder, T binder) where T : IBinder
        {
            return builder.GetSkeleton().AddInstance(binder);
        }

        public static IDISLBuilder AddBinder<T>(this IDISLBuilder builder) where T : IBinder
        {
            return builder.GetSkeleton().AddBuildingType<T>();
        }

        private static IDISLBuilderSkeleton AddBuildingType<T>(this IDISLBuilderSkeleton skeleton)
        {
            skeleton.BuildingTypes.Add(typeof(T));
            return skeleton;
        }

        private static IDISLBuilderSkeleton AddInstance(this IDISLBuilderSkeleton skeleton, object instance)
        {
            skeleton.Instances.Add(instance);
            return skeleton;
        }
    }
}