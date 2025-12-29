
namespace MeteorMadness.GlobalValues.Tools.Observer
{
    public interface IObserver
    {
        void OnNotify(ulong message, params object[] args);
    }
}

