using System;

namespace _Main.Scripts.GlobalEvents
{
    public class BootEvents
    {
        public static event Action OnMainSystemRequestInitialize;
        public static event Action OnSubSystemRequestInitialize;
        //
        public static event Action OnMainSystemInitialized;
        public static event Action OnSubSystemInitialized;
        
        public static event Action OnGameLoaded;

        public static void InitializeMainSystem()
        {
            OnMainSystemRequestInitialize?.Invoke();
        }

        public static void InitializeSubSystems()
        {
            OnSubSystemRequestInitialize?.Invoke();
        }
        
        public static void MainSystemInitialized()
        {
            OnMainSystemInitialized?.Invoke();
        }

        public static void SubSystemInitialized()
        {
            OnSubSystemInitialized?.Invoke();
        }

        public static void TriggerOnGameLoaded()
        {
            OnGameLoaded?.Invoke();
        }
    }
}