using DISL.Runtime.Utils;

namespace DISL.Runtime.Exceptions
{
    public class DISLRuntimeScriptingBackendException : DISLException
    {
        public DISLRuntimeScriptingBackendException(ScriptingBackend scriptingBackend) :
            base($"Unsupported runtime scripting backend {scriptingBackend}")
        {
        }
    }
}