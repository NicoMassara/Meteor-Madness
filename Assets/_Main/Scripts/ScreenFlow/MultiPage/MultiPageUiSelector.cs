using System;
using MeteorMadness.ScreenFlow.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.MultiPage
{
    public class MultiPageUiSelector : UiComponentsSelector<MultiPageUIComponents> { }
    
    [Serializable]
    public class MultiPageUIComponents : UiComponentsData
    {
        [Space(2)]
        [Header("Text Components")]
        public TMP_Text PanelText;
        public TMP_Text NextButtonText;
        [Header("Button Components")]
        public Button NextButton;
        public Button PreviousButton;
    }
}