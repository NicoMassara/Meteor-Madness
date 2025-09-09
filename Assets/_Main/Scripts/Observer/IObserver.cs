
namespace _Main.Scripts.Observer
{
    public interface IObserver
    {
        void OnNotify(string message, params object[] args);
    }
}

