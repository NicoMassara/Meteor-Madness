using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Localization.UI;
using _Main.Scripts.Sounds.UI;
using _Main.Scripts.Utilities;
using _Main.Scripts.Vibration.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Menu
{
    public class MainMenuUiPanelSelector : UiPanelSelector<MainMenuUiComponents> { }
    
    [Serializable]
    public class MainMenuUiComponents : UiComponentsData
    {
        [Space(2)] 
        [Header("Sub Panels")] 
        public GameObject MenuPanel;
        public GameObject LorePanel;
        public GameObject TutorialPanel;
        public GameObject CreditsPanel;
        public GameObject OptionsPanel;
        [Space]
        [Header("Text Components")]
        public TMP_Text CreditsText;
        [Space]
        [Header("Buttons Components")]
        public Button PlayButton;
        public Button TutorialButton;
        public Button OpenTutorialButton;
        public Button LoreButton;
        public Button CreditsButton;
        public Button CosmeticButton;
        public Button OptionsButton;
        public Button QuitButton;
        public Button[] BackButtons;
        [Header("Misc Components")]
        [SerializeField] private VolumeSliderUI volumeSlider;
        [SerializeField] private VibrationToggleUI vibrationToggle;
        [SerializeField] private LanguageSelectorUI languageSelector;

        public IVolumeSlider VolumeSlider => volumeSlider;
        public IVibrationToggle VibrationToggle => vibrationToggle;
        public ILanguageSelector LanguageSelector => languageSelector;
    }
}