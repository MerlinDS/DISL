namespace DISL.Runtime.Bindings
{
    public interface IBindingProvider
    {
        public Binding<T> Bind<T>();
    }
}