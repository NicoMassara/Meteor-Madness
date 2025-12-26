using System;
using System.Collections.Generic;

namespace MeteorMadness.Managers
{
    public class EventBusManager
    {
        private readonly Dictionary<Type, List<Delegate>> _listeners = new();
        
        public void Subscribe<T>(Action<T> callback)
        {
            var eventType = typeof(T);

            if (!_listeners.ContainsKey(eventType))
                _listeners[eventType] = new List<Delegate>();

            _listeners[eventType].Add(callback);
        }
        
        public void Unsubscribe<T>(Action<T> callback)
        {
            var eventType = typeof(T);

            if (_listeners.TryGetValue(eventType, out var list))
            {
                list.Remove(callback);
                if (list.Count == 0)
                    _listeners.Remove(eventType);
            }
        }
        
        public void Publish<T>(T eventData)
        {
            var eventType = typeof(T);

            if (_listeners.TryGetValue(eventType, out var list))
            {
                var listenersCopy = list.ToArray();
                foreach (var del in listenersCopy)
                {
                    if (del is Action<T> action)
                        action.Invoke(eventData);
                }
            }
        }

        public void Clear()
        {
            _listeners.Clear();
        }
    }
}