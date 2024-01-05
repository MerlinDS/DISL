using DISL.Runtime.Base;

namespace DISL.Runtime.Installers
{
    public interface IBindingInstaller
    {
        void Install(IContainerBuilder builder);
    }
}