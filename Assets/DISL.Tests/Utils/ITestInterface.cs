using System;

namespace DISL.Tests.Utils
{
    internal interface ITestInterface : IDisposable
    {
            
    }
    
    internal abstract class TestAbstractImplementation : ITestInterface
    {
        /// <inheritdoc />
        public abstract void Dispose();
    }
    
    internal class TestConcreteImplementation : TestAbstractImplementation
    {
        /// <inheritdoc />
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}