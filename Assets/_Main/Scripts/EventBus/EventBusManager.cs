using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.EventBus
{
    public class EventBusManager : MonoBehaviour
    {
        #region Singleton
        public static EventBusManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        protected static EventBusManager _instance;
        
        private static EventBusManager CreateInstance()
        {
            var gameObject = new GameObject(typeof(EventBusManager).ToString())
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            //SingletonEvents.OnDestroySingleton += () => DestroyImmediate(gameObject);
            return gameObject.AddComponent<EventBusManager>();
        }

        public static void LoadInstance() => Instance.Internal_LoadInstance();

        private void Internal_LoadInstance(){ }
        
        #endregion
        
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