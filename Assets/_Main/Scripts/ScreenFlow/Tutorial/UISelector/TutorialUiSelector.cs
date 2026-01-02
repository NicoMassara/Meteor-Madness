using System;
using MeteorMadness.ScreenFlow.Base;
using TMPro;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Tutorial
{
    public class TutorialUiSelector : UiComponentsSelector<TutorialUiComponents> { }
    
    [Serializable]
    public class TutorialUiComponents : UiComponentsData
    {
        [Space(2)] 
        [Space]
        [Header("Text Components")]
        public TMP_Text HintText;
    }
}