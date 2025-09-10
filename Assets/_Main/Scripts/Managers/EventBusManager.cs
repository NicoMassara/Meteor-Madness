using System;
using System.Collections.Generic;

namespace _Main.Scripts.Managers
{
    public class EventBusManager
    {
        private readonly Dictionary<Type, List<Action<object>>> _listeners = new();
        
        public void Subscribe<T>(Action<T> callback)
        {
            Type eventType = typeof(T);
            if (!_listeners.ContainsKey(eventType))
                _listeners[eventType] = new List<Action<object>>();

            _listeners[eventType].Add((obj) => callback((T)obj));
        }
        
        public void Unsubscribe<T>(Action<T> callback)
        {
            Type eventType = typeof(T);
            if (_listeners.ContainsKey(eventType))
                _listeners[eventType].RemoveAll(a => a.Equals((Action<object>)(obj => callback((T)obj))));
        }
        
        public void Publish<T>(T eventData)
        {
            Type eventType = typeof(T);
            if (_listeners.ContainsKey(eventType))
            {
                foreach (var listener in _listeners[eventType])
                    listener.Invoke(eventData);
            }
        }
    }
    
    
}