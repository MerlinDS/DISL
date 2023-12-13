using DISL.Runtime.Builders;

namespace DISL.Runtime.Exceptions
{
    public class DISLInvalidBuilderException : DISLException
    {
        /// <inheritdoc />
        public DISLInvalidBuilderException(IContainerBuilder builder) : base($"Invalid container builder {builder}")
        {
        }
        
        public DISLInvalidBuilderException(IDISLBuilder builder) : base($"Invalid DISL builder {builder}")
        {
        }
    }
}