using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUiPanelSelector : UiComponentsSelector<GameModeUIComponents> { }
    
    [Serializable]
    public class GameModeUIComponents : UiComponentsData
    {
        [Space(2)] 
        [Header("Text")]
        public TMP_Text CountdownText;
        public TMP_Text ScoreText;
        [Header("Buttons Components")]
        public Button PauseButton;
    }
}