using System;
using MeteorMadness.ScreenFlow.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.GameMode
{
    public class GameModeUiPanelSelector : UiComponentsSelector<GameModeUIComponents> { }
    
    [Serializable]
    public class GameModeUIComponents : UiComponentsData
    {
        [Space(2)] 
        [Header("Text")]
        public TMP_Text CountdownText;
        public TMP_Text ScoreText;
        public TMP_Text StreakText;
        [SerializeField] private TMP_Text notifyText;
        [Header("Buttons Components")]
        public Button PauseButton;
        
        public void SetScoreText(string textCode, uint amount) 
            => SetText(ScoreText, $"{textCode}: {amount:D6}");
        
        public void SetStreakText(string textCode, uint amount) 
            => SetText(StreakText, $"{textCode}: {amount:D4}");
        public void SetCountdownText(float countdownTime)
        {
            var stringValue = "";
            
            if (countdownTime > 0)
            {
                stringValue = $"{GetLocalizedString("Gameplay.CountDownStart")} {(int)countdownTime}...";
            }
            else
            {
                stringValue = GetLocalizedString("Gameplay.CountdownFinish");
            }

            SetText(CountdownText, stringValue);
        }

        public void SetNotifyText(string textCode) 
            => SetText(notifyText, textCode);
        
    }
}