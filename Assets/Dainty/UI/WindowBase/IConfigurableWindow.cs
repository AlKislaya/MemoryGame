namespace Dainty.UI.WindowBase
{
    public interface IConfigurableWindow<in T>
    {
        void Initialize(T data);
    }
}