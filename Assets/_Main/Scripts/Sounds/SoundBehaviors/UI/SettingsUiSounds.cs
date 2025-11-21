using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Sounds;

namespace _Main.Scripts.Sounds.Components
{
    public class SettingsUiSounds : UiSoundBehavior<ISettingsUISounds>
    {
        private void Start()
        {
            GetComponentToSound.OnVolumeChanged += (value) =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
            GetComponentToSound.OnLanguageChanged += (value) =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            
#if UNITY_ANDROID && !UNITY_EDITOR
            
            GetComponentToSound.OnVibrationChanged += (value) =>
            {
                PlayUISound(UISoundType.Confirm);
            };

#endif
            
            GetComponentToSound.OnBackButtonPressed += () =>
            {
                PlayUISound(UISoundType.Back);
            };
        }
    }
}