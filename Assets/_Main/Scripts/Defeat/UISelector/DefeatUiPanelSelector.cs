using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Main.Scripts.Defeat
{
    [AddComponentMenu("_Main/Defeat/UI Selector")]
    public class DefeatUiPanelSelector : UiComponentsSelector<DefeatUIComponents> { }
    
    [Serializable]
    public class DefeatUIComponents : UiComponentsData
    {
        [Header("Text Components")] 
        [SerializeField] private TMP_Text deathTitle;
        [SerializeField] private TMP_Text storedCoinsText;
        [SerializeField] private TMP_Text newCoinsText;
        [Header("Buttons Components")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        public void SetStoredCoinsText(string textCode, uint coinsAmount)
            => SetText(storedCoinsText, $"{GetLocalizedString(textCode)}: {coinsAmount:D4}");
        
        public void SetNewCoinsText(string textCode, uint coinsAmount)
            => SetText(newCoinsText, $"{GetLocalizedString(textCode)}: {coinsAmount:D2}");

        public void SetDeathTitle(string textCode) 
            => SetText(deathTitle, GetLocalizedString(textCode));
        
        public void RestartButton_AddListener(UnityAction onclick)
            => AddListenerToButton(restartButton, onclick);
        public void RestartButton_RemoveListener(UnityAction onclick)
            => RemoveListenerFromButton(restartButton, onclick);
        
        public void MainMenuButton_AddListener(UnityAction onclick)
            => AddListenerToButton(mainMenuButton, onclick);
        public void ainMenuButton_RemoveListener(UnityAction onclick)
            => RemoveListenerFromButton(mainMenuButton, onclick);
    }
    
}