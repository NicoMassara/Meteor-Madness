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
        [Header("Sub Panels")] 
        public GameObject MenuPanel;
        public GameObject LorePanel;
        public GameObject TutorialPanel;
        public GameObject CreditsPanel;
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
        public Button QuitButton;
        public Button[] BackButtons;
    }
}