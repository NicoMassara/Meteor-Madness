using System;

namespace MeteorMadness.GlobalEvents.Events
{
    public class BootEvents
    {
        public static event Action OnMainSystemRequestInitialize;
        
        public static void InitializeMainSystem()
        {
            OnMainSystemRequestInitialize?.Invoke();
        }
        
        // ============================ //
        
        public static event Action OnSubSystemRequestInitialize;
        public static void InitializeSubSystems()
        {
            OnSubSystemRequestInitialize?.Invoke();
        }
        
        // ============================ //
        
        public static event Action OnMainSystemInitialized;
        
        public static void MainSystemInitialized()
        {
            OnMainSystemInitialized?.Invoke();
        }
        
        // ============================ //
        public static event Action OnSubSystemInitialized;
        
        public static void SubSystemInitialized()
        {
            OnSubSystemInitialized?.Invoke();
        }
        
        // ============================ //
        
        public static void TriggerOnGameLoaded()
        {
            OnGameLoaded?.Invoke();
        }
        public static event Action OnGameLoaded;




        





    }
}