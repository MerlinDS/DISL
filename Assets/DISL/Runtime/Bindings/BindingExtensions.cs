namespace DISL.Runtime.Bindings
{
    /// <summary>
    /// The extension methods for binding API.
    /// </summary>
    public static class BindingExtensions
    {
        public static void ToInstance<TType, TImplementation>(this in Binding<TType> binding,
            TImplementation instance) where TImplementation : TType, new()
        {
            // binding.To<TImplementation>().ToInstance(instance);
        }
        
        public static void AsSingle<TType, TImplementation>(this in Binding<TType> binding)
            where TImplementation : TType, new()
        {
            // binding.To<TImplementation>().AsSingle();
        }
    }
}