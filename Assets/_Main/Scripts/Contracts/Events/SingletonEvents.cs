using System;

namespace MeteorMadness.Contracts.Events
{
    public class SingletonEvents
    {
        public static Action OnDestroySingleton;
        public static void DestroySingleton() => OnDestroySingleton?.Invoke();
        
    }
}