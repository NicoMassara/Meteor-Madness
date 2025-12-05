using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Localization;
using _Main.Scripts.Observer;
using _Main.Scripts.ViewUI;
using UnityEngine;

namespace _Main.Scripts.Pause
{
    public class PauseViewUI : BaseViewUI<PauseUIComponentsSelector,PauseUIComponents>,
        PauseViewUI.IPauseViewUI,
        IPausePanelUISounds
    {
        public interface IPauseViewUI
        {
            public event Action OnResumeButtonPressed;
            public event Action OnOptionsButtonPressed;
            public event Action OnMainMenuButtonPressed;
        }
        
        public event Action OnResumeButtonPressed;
        public event Action OnOptionsButtonPressed;
        public event Action OnMainMenuButtonPressed;

        
        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
               case PauseObserverMessage.Enable:
                   HandleEnable();
                   break;
               case PauseObserverMessage.StartDisable:
                   HandleStartDisable();
                   break;
            }
        }
        

        public void SetScoreText(int points)
        {
            if(UIComponents.ScoreText == null) return;

            UIComponents.ScoreText.text = $"{LocalizationManager.Instance.GetText("Gameplay.Score")}: {points:D6}";
        }
        
        private void HandleEnable()
        {
            UIComponents.ResumeButton?.onClick.AddListener(() =>
            {
                OnResumeButtonPressed?.Invoke();
            });
            
            UIComponents.OptionsButton?.onClick.AddListener(() =>
            {
                OnOptionsButtonPressed?.Invoke();
            });
            
            UIComponents.MainMenuButtons?.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
            });
        }
        private void HandleStartDisable()
        {
            UIComponents.ResumeButton.onClick.RemoveAllListeners();
            UIComponents.OptionsButton.onClick.RemoveAllListeners();
            UIComponents.MainMenuButtons.onClick.RemoveAllListeners();
        }
    }
}