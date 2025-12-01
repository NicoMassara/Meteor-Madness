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
        [Header("Panels")] 
        public GameObject HintPanel;
        [Space]
        [Header("Text Components")]
        public TMP_Text HintText;
    }
}