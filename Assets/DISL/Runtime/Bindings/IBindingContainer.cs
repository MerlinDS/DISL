namespace DISL.Runtime.Bindings
{
    public interface IBindingContainer
    {
        Binding<T> Bind<T>();
    }
}