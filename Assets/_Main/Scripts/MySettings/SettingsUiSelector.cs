using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Localization.UI;
using _Main.Scripts.Sounds.UI;
using _Main.Scripts.Utilities;
using _Main.Scripts.Vibration.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MySettings
{
    public class SettingsUiSelector :  UiPanelSelector<SettingsUiComponents> { }
    
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
        public LanguageSelectorUI LanguageSelector => LanguageSelector;
        public VolumeSliderUI VolumeSlider => VolumeSlider;
        public VibrationToggleUI VibrationToggle => VibrationToggle;
    }
}