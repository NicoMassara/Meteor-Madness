using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
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
        [Header("Panels")] 
        public GameObject MenuPanel;
        public GameObject LorePanel;
        public GameObject TutorialPanel;
        public GameObject CreditsPanel;
        [Space]
        [Header("Text Components")]
        public TMP_Text CreditsText;
        [Space]
        [Header("Buttons Components")]
        [Space(1)]
        [Header("Menu Panel")]
        public Button PlayButton;
        public Button TutorialButton;
        public Button CosmeticButton;
        public Button LoreButton;
        public Button OptionsButton;
        public Button CreditsButton;
        public Button QuitButton;
        [Space(1)]
        [Header("Tutorial Panel")]
        public Button OpenTutorialButton;
        [Space(1)]
        [Header("Back")]
        public Button[] BackButtons;
    }
}