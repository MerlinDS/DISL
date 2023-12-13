using System;

namespace DISL.Runtime.Exceptions
{
    /// <summary>
    /// The base class for all exceptions thrown by the DISL.
    /// </summary> 
    public class DISLException : Exception
    {
        /// <inheritdoc />
        public DISLException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DISLException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception.</param>
        public DISLException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}