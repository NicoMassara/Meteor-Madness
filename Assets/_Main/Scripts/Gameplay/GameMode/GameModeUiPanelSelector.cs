using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUiPanelSelector : UiPanelSelector<GameModeUIComponents> { }
    
    [Serializable]
    public class GameModeUIComponents : UiComponentsData
    {
        [Space(2)] 
        [Header("Sub Panels")] 
        public GameObject PausePanel;
        public GameObject CountdownPanel;
        public GameObject GameplayPanel;
        public GameObject DeathPanel;
        public GameObject DeathButtonContainer;
        [Space]
        [Header("Texts Components")]
        public TMP_Text CountdownText;
        public TMP_Text ScoreText;
        public TMP_Text DeathScoreText;
        public TMP_Text DeathText;
        [Space]
        [Header("Buttons Components")]
        public Button RestartButton;
        public Button ResumeButton;
        public Button PauseButton;
        public Button[] MainMenuButtons;
    }
}