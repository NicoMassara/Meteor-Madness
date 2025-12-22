using System;

namespace _Main.Scripts.MyComponents
{
    public class SingletonEvents
    {
        public static Action OnDestroySingleton;
        public static void DestroySingleton() => OnDestroySingleton?.Invoke();
        
    }
}