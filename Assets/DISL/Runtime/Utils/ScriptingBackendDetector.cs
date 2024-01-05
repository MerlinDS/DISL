namespace DISL.Runtime.Utils
{
    public static class ScriptingBackendDetector
    {
        public static ScriptingBackend Detect()
        {
            if(IsIL2CPPEnabled()) 
                return ScriptingBackend.IL2CPP;
             
            if (IsMonoEnabled()) 
                return ScriptingBackend.Mono;
 
            return ScriptingBackend.Undefined;
        }
 
        private static bool IsIL2CPPEnabled()
        {
#if ENABLE_IL2CPP
             return true;
#else
            return false;
#endif
        }
     
        private static bool IsMonoEnabled()
        {
#if ENABLE_MONO
            return true;
#else
             return false;
#endif
        }
    }
}