using System;

namespace MeteorMadness.Contracts.Events
{
    public class SkinEvents
    {
        public static event Action OnAssetsLoaded;
        public static event Action OnSaveLoaded;

        public static void TriggerOnAssetsLoaded()
        {
            OnAssetsLoaded?.Invoke();
        }
        
        public static void TriggerOnSaveLoaded()
        {
            OnSaveLoaded?.Invoke();
        }
    }
}