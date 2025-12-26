
using MeteorMadness.GlobalValues.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;

namespace MeteorMadness.Sounds.SoundBehaviors
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