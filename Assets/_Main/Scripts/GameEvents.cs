using System;

namespace _Main.Scripts
{
    public class GameEvents
    {
        public static event Action OnGameLoaded;
        
        public static void TriggerOnGameLoaded()
        {
            OnGameLoaded?.Invoke();
        }
    }
}