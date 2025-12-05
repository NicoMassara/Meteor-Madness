using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Localization;
using _Main.Scripts.Observer;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIView : ManagedBehavior, IObserver, IGameModeUISounds,
        GameModeUIView.IGameModeUIView
    {
        public interface IGameModeUIView
        {
            public event Action OnPauseButtonPressed;
        }

        [SerializeField] private GameModeUiPanelSelector uiSelector;
        
        private GameModeUIComponents _uiComponents;
        private Coroutine _gameplayPointsCoroutine;
        
        private bool _hasHighScore;
        private float _highScore;
        private string _scoreTextValue;
        
        public event Action OnPauseButtonPressed;

        private GameModeUIComponents GetUiComponents()
        {
            return _uiComponents ??= _uiComponents = uiSelector.GetPanelData();
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                //=== Gameplay ===//
                case GameModeObserverMessage.EnableGameplayUI:
                    HandleEnableGameplayUI();
                    break;
                case GameModeObserverMessage.DisableGameplayUI:
                    HandleDisableGameplayUI();
                    break;
                
                //=== Countdown ==//
                case GameModeObserverMessage.UpdateCountdown:
                    HandleUpdateCountdown((float)args[0]);
                    break;
            }
        }

        private void HandleUpdateCountdown(float countdownTime)
        {
            var text = countdownTime >= 1 ? $"{GetLocalizedString("Gameplay.CountDownStart")} {(int)countdownTime}..." 
                : GetLocalizedString("Gameplay.CountdownFinish");
            GetUiComponents().CountdownText.text = text;
        }
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }

        #region Gameplay UI

        private void HandleEnableGameplayUI()
        {
            GetUiComponents().PauseButton.onClick.AddListener(() =>
            {
                OnPauseButtonPressed?.Invoke();
            });
        }
        
        private void HandleDisableGameplayUI()
        {
            GetUiComponents().PauseButton.onClick.RemoveAllListeners();
        }

        #endregion
        
    }
}