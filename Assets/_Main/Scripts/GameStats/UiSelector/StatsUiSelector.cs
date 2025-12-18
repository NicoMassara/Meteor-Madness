using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.GameStats
{
    public class StatsUiSelector : UiComponentsSelector<StatsUiComponents> { }
    
    [Serializable]
    public class StatsUiComponents : UiComponentsData
    {
        [Header("Text Components")] 
        [SerializeField] private TMP_Text deflectAmountText;
        [SerializeField] private TMP_Text collisionAmountText;
        [SerializeField] private TMP_Text abilityUseAmountText;
        [SerializeField] private TMP_Text gamesPlayedText;
        [SerializeField] private TMP_Text deflectStreakText;
        [SerializeField] private TMP_Text longestTimeText;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private TMP_Text allScoreText;
        [Space]
        [Header("Buttons")]
        [SerializeField] private Button backButton;

        
        #region Texts
        
        public void SetDeflectAmountText(string langKey, uint value) => SetText(deflectAmountText, $"{GetLocalizedString(langKey)}: {value:D8}");
        public void SetCollisionAmountText(string langKey, uint value) => SetText(collisionAmountText, $"{GetLocalizedString(langKey)}: {value:D8}");
        public void SetAbilityUseAmountText(string langKey, uint value) => SetText(abilityUseAmountText, $"{GetLocalizedString(langKey)}: {value:D8}");
        public void SetGamesPlayedText(string langKey, uint value) => SetText(gamesPlayedText, $"{GetLocalizedString(langKey)}: {value:D8}");
        public void SetDeflectStreakText(string langKey, uint value) => SetText(deflectStreakText, $"{GetLocalizedString(langKey)}: {value:D8}");
        public void SetLongestTimeText(string langKey, float value) => SetText(longestTimeText, $"{GetLocalizedString(langKey)}: {FormatToHMS(value)}");
        public void SetHighScoreText(string langKey, uint value) => SetText(highScoreText, $"{GetLocalizedString(langKey)}: {value:D8}");
        public void SetAllScoreText(string langKey, long value) => SetText(allScoreText, $"{GetLocalizedString(langKey)}: {value:D8}");
        
        #endregion

        #region Buttons

        public void AddBackButtonListener(UnityAction onClick) => AddListenerToButton(backButton, onClick);
        public void RemoveBackButtonListener(UnityAction onClick) => RemoveListenerFromButton(backButton, onClick);

        #endregion
        
        private string FormatToHMS(float totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            string value = null;

            if (totalSeconds <= 0)
            {
                value = "---";
                return value;
            }
            
            if (time.Hours > 0)
            {
                value += $"{(int)time.TotalHours}hs ";
            }
            
            if (time.Minutes > 0)
            {
                value += $"{time.Minutes:D2}m ";
            }
            
            if (time.Seconds > 0)
            {
                value += $"{time.Seconds:D2}s";
            }
            
            return value;
        }
    }
}