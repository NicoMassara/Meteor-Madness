using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;

namespace _Main.Scripts.Sounds.Components
{
    public class SettingsUiSounds : UiSoundBehavior<ISettingsUISounds>
    {
        private void Start()
        {
            ComponentToSound.OnVolumeChanged += (value) =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            ComponentToSound.OnLanguageChanged += (value) =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
#if UNITY_ANDROID
            
            ComponentToSound.OnVibrationChanged += (value) =>
            {
                PlayUISound(UISoundType.Confirm);
            };

#endif
            
            ComponentToSound.OnBackButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}