using System;

namespace _Main.Scripts
{
    public class BootEvents
    {
        public static event Action OnMainSystemRequestInitialize;
        public static event Action OnSubSystemRequestInitialize;
        //
        public static event Action OnMainSystemInitialized;
        public static event Action OnSubSystemInitialized;
        
        public static event Action OnGameLoaded;

        public static void TriggerOnMainSystemRequestInitialize()
        {
            OnMainSystemRequestInitialize?.Invoke();
        }

        public static void TriggerOnMainSubSystemRequestInitialize()
        {
            OnSubSystemRequestInitialize?.Invoke();
        }
        
        public static void TriggerOnMainSystemInitialized()
        {
            OnMainSystemInitialized?.Invoke();
        }

        public static void TriggerOnSubSystemInitialized()
        {
            OnSubSystemInitialized?.Invoke();
        }

        public static void TriggerOnGameLoaded()
        {
            OnGameLoaded?.Invoke();
        }
    }
}