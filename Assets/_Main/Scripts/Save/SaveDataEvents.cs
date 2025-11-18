using System;

namespace _Main.Scripts.Save
{
    public class SaveDataEvents
    {
        public static event Action OnSaveInitialized;
        
        public static void TriggerOnSaveInitialized()
        {
            OnSaveInitialized?.Invoke();
        }
    }
}