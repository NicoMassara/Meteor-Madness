using System;
using _Main.Scripts.Pause;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.GlobalValues.Interfaces.Sounds;
using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Localization;
using MeteorMadness.ScreenFlow.Base;

namespace MeteorMadness.ScreenFlow.Pause
{
    public class PauseUIView : BaseViewUI<PauseUIComponentsSelector,PauseUIComponents>,
        PauseUIView.IPauseViewUI,
        IPausePanelUISounds, IPauseUIVibration
    {
        public interface IPauseViewUI
        {
            public event Action OnResumeButtonPressed;
            public event Action OnOptionsButtonPressed;
            public event Action OnMainMenuButtonPressed;
        }

        private string _localizedScoreText;
        
        public event Action OnResumeButtonPressed;
        public event Action OnOptionsButtonPressed;
        public event Action OnMainMenuButtonPressed;


        private void Start()
        {
            LocalizationEvents_OnLanguageChangedHandler();
            LocalizationEvents.OnLanguageChanged += LocalizationEvents_OnLanguageChangedHandler;
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case PauseObserverMessage.Initialize:
                    HandleInitialize();
                    break;
               case PauseObserverMessage.Enable:
                   HandleEnable();
                   break;
               case PauseObserverMessage.StartDisable:
                   HandleStartDisable();
                   break;
            }
        }



        #region Score Text
        
        private void SetScoreText(uint points)
        {
            if(UIComponents.ScoreText == null) return;
            
            UIComponents.ScoreText.text = $"{_localizedScoreText}: {points:D6}";
        }

        private void UpdateLocalizedScoreText()
        {
            _localizedScoreText = LocalizationManager.Instance.GetText("Gameplay.Score");
        }
        
        private void LocalizationEvents_OnLanguageChangedHandler()
        {
            UpdateLocalizedScoreText();
            SetScoreText(GameManager.Instance.StatsController.VisualPoints);
        }
        
        #endregion

        private void HandleInitialize()
        {
            SetScoreText(GameManager.Instance.StatsController.VisualPoints);
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