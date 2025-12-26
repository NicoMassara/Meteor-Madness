using System;

namespace MeteorMadness.GlobalValues.Events
{
    public class SingletonEvents
    {
        public static Action OnDestroySingleton;
        public static void DestroySingleton() => OnDestroySingleton?.Invoke();
        
    }
}