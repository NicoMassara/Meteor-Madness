
namespace _Main.Scripts.Observer
{
    public interface IObserver
    {
        void OnNotify(ulong message, params object[] args);
    }
}

