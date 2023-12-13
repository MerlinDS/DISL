using DISL.Runtime.Base;

namespace DISL.Runtime.Installers
{
    public interface IInstaller
    {
        void Install(IResolvingContainer container);
    }
}