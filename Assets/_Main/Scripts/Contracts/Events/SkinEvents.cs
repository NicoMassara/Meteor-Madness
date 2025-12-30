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
        
        // ================================================== //
        
        public static event Action<string> OnSkinChanged;
        public static void TriggerOnSkinChanged(string skinName) => OnSkinChanged?.Invoke(skinName);
    }
}