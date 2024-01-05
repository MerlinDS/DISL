using DISL.Runtime.Base;

namespace DISL.Runtime.Installers
{
    public interface IContainerInstaller
    {
        void Install(IContainer container);
    }
}