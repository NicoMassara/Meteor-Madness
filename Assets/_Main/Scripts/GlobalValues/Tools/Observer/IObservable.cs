using System.Collections.Generic;

namespace MeteorMadness.GlobalValues.Tools.Observer
{
    public interface IObservable
    {
        List<IObserver> Subscribers { get; }

        void Subscribe(IObserver observer);
        void Unsubscribe(IObserver observer);

        void NotifyAll(ulong message, params object[] args);
    }
}

