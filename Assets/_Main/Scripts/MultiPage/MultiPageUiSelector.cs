using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageUiSelector : UiPanelSelector<MultiPageUIComponents> { }
    
    [Serializable]
    public class MultiPageUIComponents : UiComponentsData
    {
        [Space(2)]
        [Header("Panels")]
        public GameObject MainPanel;
        [Header("Text Components")]
        public TMP_Text PanelText;
        public TMP_Text NextButtonText;
        [Header("Button Components")]
        public Button NextButton;
        public Button PreviousButton;
    }
}