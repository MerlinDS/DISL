using DISL.Runtime.Builders;

namespace DISL.Runtime.Base
{
    public interface IContainerConfigurator
    {
        IContainerBuilder Configure(IContainerBuilder builder);
    }
}