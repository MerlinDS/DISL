using System;

namespace DISL.Runtime.Exceptions
{
    public class DISLInvalidConstructionException : DISLException
    {
        /// <inheritdoc />
        public DISLInvalidConstructionException(Type type, Exception inner) :
            base($"Can't create instance of {type}", inner)
        {
        }
    }
}