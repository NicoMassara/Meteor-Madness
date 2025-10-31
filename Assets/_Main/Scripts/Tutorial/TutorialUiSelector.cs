using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Tutorial
{
    public class TutorialUiSelector : UiPanelSelector<TutorialUiComponents> { }
    
    [Serializable]
    public class TutorialUiComponents : UiComponentsData
    {
        [Space(2)] 
        [Header("Sub Panels")] 
        public GameObject StartPanel;
        public GameObject HintPanel;
        [Space]
        [Header("Buttons Components")]
        public Button StartButton;
        public Button MainMenuButton;
        [Space]
        [Header("Text Components")]
        public TMP_Text HintText;
    }
}