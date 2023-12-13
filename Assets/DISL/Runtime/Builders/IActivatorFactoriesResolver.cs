using DISL.Runtime.Reflections;
using DISL.Runtime.Utils;

namespace DISL.Runtime.Builders
{
    public interface IActivatorFactoriesResolver
    {
        IActivatorFactory Resolve(ScriptingBackend scriptingBackend);
    }
}