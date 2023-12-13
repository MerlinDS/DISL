using System;

namespace DISL.Runtime.Exceptions
{
    /// <summary>
    /// Exception for when a type is not registered in the DI container.
    /// </summary>
    public class DISLBindingRegistrationException : DISLException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DISLBindingRegistrationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DISLBindingRegistrationException(string message) : base(message)
        {
        }
        
        public DISLBindingRegistrationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}