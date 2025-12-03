using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Tutorial
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