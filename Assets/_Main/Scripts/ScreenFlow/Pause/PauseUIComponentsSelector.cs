using System;
using MeteorMadness.ScreenFlow.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Pause
{
    public class PauseUIComponentsSelector : UiComponentsSelector<PauseUIComponents> { }
    
    
    [Serializable]
    public class PauseUIComponents : UiComponentsData
    {
        [Space(2)] 
        [Header("Texts Components")]
        public TMP_Text ScoreText;
        [Space]
        [Header("Buttons Components")]
        public Button ResumeButton;
        public Button OptionsButton;
        public Button MainMenuButtons;
    }
}