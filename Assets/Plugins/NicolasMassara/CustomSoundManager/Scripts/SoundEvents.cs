using System;

namespace Plugins.NicolasMassara.CustomSoundManager
{
    public class SoundEvents
    {
        #region Boot
        
        public static event Action OnInitializeSoundManager;
        public static void InitializeSoundManager()
        {
            OnInitializeSoundManager?.Invoke();
        }
        
        // =================================== // 
        
        public static event Action OnSoundManagerInitialized;
        public static void SoundManagerInitialized()
        {
            OnSoundManagerInitialized?.Invoke();
        }
        
        #endregion
    }
}