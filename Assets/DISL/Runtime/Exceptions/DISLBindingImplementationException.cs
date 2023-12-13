using System;

namespace DISL.Runtime.Exceptions
{
    public class DISLBindingImplementationException : DISLBindingRegistrationException
    {
        public static void Guard(Type bindingType, Type implementationType)
        {
            ValidateTypeAssignability(bindingType, implementationType);
            ValidateTypeNonAbstract(implementationType);
        }

        private static void ValidateTypeAssignability(Type bindingType, Type implementationType)
        {
            if (!bindingType.IsAssignableFrom(implementationType))
            {
                throw new DISLBindingImplementationException(
                    $"Type {implementationType} is not assignable from {bindingType}.");
            }
        }

        private static void ValidateTypeNonAbstract(Type implementationType)
        {
            if (implementationType.IsAbstract)
            {
                throw new DISLBindingImplementationException(
                    $"Type {implementationType} is abstract and can't be used as implementation.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DISLBindingRegistrationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public DISLBindingImplementationException(string message) : base(message)
        {
        }
    }
}