using System.Collections.Generic;

namespace _Main.Scripts.Observer
{
    public class ObservableComponent : IObservable
    {
        public List<IObserver> Subscribers { get; private set; }
        
        public void Subscribe(IObserver observer)
        {
            Subscribers ??= new List<IObserver>();
            
            if (!Subscribers.Contains(observer))
            {
                Subscribers.Add(observer);
            }
        }

        public void Unsubscribe(IObserver observer)
        {
            Subscribers ??= new List<IObserver>();
            
            if (Subscribers.Contains(observer))
            {
                Subscribers.Remove(observer);
            }
        }

        public void NotifyAll(string message, params object[] args)
        {
            Subscribers ??= new List<IObserver>();
            
            foreach (var sus in Subscribers)
            {
                sus.OnNotify(message, args);
            }
        }
    }
}