using System;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.ScreenFlow.Base;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.Settings
{
    public class SettingsUiSelector :  UiComponentsSelector<SettingsUiComponents> { }
    
    [Serializable]
    public class SettingsUiComponents : UiComponentsData
    {
        [Space]
        [Header("Buttons")]
        public Button BackButton;
        [Header("Components")]
        [SerializeField] private LanguageSelectorUI languageSelector;
        [SerializeField] private VolumeSliderUI volumeSlider;
        [SerializeField] private VibrationToggleUI vibrationToggle;
        public ILanguageSelector LanguageSelector => languageSelector;
        public IVolumeSlider VolumeSlider => volumeSlider;
        public IVibrationToggle VibrationToggle => vibrationToggle;
    }
}