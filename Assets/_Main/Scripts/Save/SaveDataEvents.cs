using System;

namespace _Main.Scripts.Save
{
    public class SaveDataEvents
    {
        public static event Action OnSaveInitialized;
        public static event Action OnSaveDataCorrupted;
        
        public static void TriggerOnSaveInitialized()
        {
            OnSaveInitialized?.Invoke();
        }

        public static void TriggerOnSaveDataCorrupted()
        {
            OnSaveDataCorrupted?.Invoke();
        }
    }
}