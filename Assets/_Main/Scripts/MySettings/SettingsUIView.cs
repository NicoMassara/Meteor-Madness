using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Localization.UI;
using _Main.Scripts.Sounds.UI;
using _Main.Scripts.Vibration.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MySettings.UI
{
    public class SettingsUIView : MonoBehaviour
    {
        [SerializeField] private VolumeSliderUI volumeSlider;
        [SerializeField] private VibrationToggleUI vibrationToggle;
        [SerializeField] private LanguageSelectorUI languageSelector;
        [SerializeField] private Button backButton;

        public IVolumeSlider VolumeSlider => volumeSlider;
#if UNITY_ANDROID
        
        public IVibrationToggle VibrationToggle => vibrationToggle;
#endif
        public ILanguageSelector LanguageSelector => languageSelector;
        
        public event Action OnBackButtonPressed;

        private void OnEnable()
        {
            backButton.onClick.AddListener(BackButton_OnClickHandler);
        }

        private void OnDisable()
        {
            backButton.onClick.RemoveListener(BackButton_OnClickHandler);
        }

        private void BackButton_OnClickHandler()
        {
            OnBackButtonPressed?.Invoke();
        }
    }
}