using _Main.Scripts.MySettings.UI;
using _Main.Scripts.Sounds;

namespace _Main.Scripts.MySettings.Components
{
    public class SettingsUiSounds : SoundBehaviour<SettingsUIView>
    {
        private void Start()
        {
            GetComponentToSound.VolumeSlider.OnChanged += (value) =>
            {
                PlayUISound(UISoundType.Confirm);
            };
            GetComponentToSound.LanguageSelector.OnChanged += (value) =>
            {
                PlayUISound(UISoundType.Confirm);
            };
#if UNITY_ANDROID
            GetComponentToSound.VibrationToggle.OnChanged += (value) =>
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