namespace DISL.Runtime.Base
{
    public interface IResolvingContainer
    {
        /// <summary>
        /// Resolves the instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the instance to resolve.</typeparam>
        /// <returns>The instance of the specified type.</returns>
        T Resolve<T>();
    }
}