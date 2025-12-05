using System;
using System.Globalization;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.MyAnimations;
using _Main.Scripts.Observer;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeViewAnimation : BaseViewAnimation<GameModeUIAnimationSelector,GameModeUIAnimationComponents>,
        GameModeViewAnimation.IGameModeViewAnimation
    {
        public interface IGameModeViewAnimation : BaseViewAnimation<GameModeUIAnimationSelector,GameModeUIAnimationComponents>.IBaseViewAnimation
        {
            
        }
        
        private string _scoreTextValue;
        private int _storedPoints;
        
        private void Start()
        {
            _scoreTextValue = GetLocalizedString("Gameplay.Score");
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                //=== Data ===//
                case GameModeObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
                
                //=== Meteor ===//
                case GameModeObserverMessage.PointsGained:
                    HandlePointsGained((float)args[1]);
                    break;
                
                //=== Countdown ===//
                case GameModeObserverMessage.StartCountdown:
                    HandleStartCountdown((int)args[0]);
                    break;
                case GameModeObserverMessage.UpdateCountdown:
                    HandleUpdateCountdown((float)args[0]);
                    break;
                case GameModeObserverMessage.FinishCountdown:
                    HandleFinishCountdown();
                    break;
                
                //=== Gameplay ===//
                case GameModeObserverMessage.EnableGameplayUI:
                    HandleEnableGameplayUI();
                    break;
                case GameModeObserverMessage.DisableGameplayUI:
                    HandleDisableGameplayUI();
                    break;
                
                //=== Internal Level ===//
                case GameModeObserverMessage.UpdateGameLevel:
                    HandleUpdateGameLevel((int)args[0]);
                    break;
            }
        }

        #region Data

        private void HandleInitializeData()
        {
            _storedPoints = 0;
        }

        #endregion

        #region Meteor

        private void HandlePointsGained(float amount)
        {
            var targetPoints =  _storedPoints + (int)(amount * GetPointsMultiplier());
            
            var textValue = $"{GetLocalizedString("Gameplay.Score")}:{targetPoints:D6}";
            
            _storedPoints = targetPoints;

            UIComponents.ScoreText.GetComponent<TMP_Text>().text = textValue;
        }

        #endregion
        
        #region Countdown

        private void HandleStartCountdown(int delay)
        {
            UIComponents.CountdownPanel.gameObject.SetActive(true);
        }
        
        private void HandleUpdateCountdown(float time)
        {
        }
        
        private void HandleFinishCountdown()
        {
            UIComponents.CountdownPanel.gameObject.SetActive(false);
        }

        #endregion
        
        #region Gameplay UI

        private void HandleEnableGameplayUI()
        {
            UIComponents.GameplayPanel.gameObject.SetActive(true);
            UIComponents.PauseButton.gameObject.SetActive(true);
            UIComponents.ScoreText.gameObject.SetActive(true);
        }
        
        private void HandleDisableGameplayUI()
        {
            UIComponents.GameplayPanel.gameObject.SetActive(false);
            UIComponents.ScoreText.gameObject.SetActive(false);
            UIComponents.PauseButton.gameObject.SetActive(false);
        }


        #endregion
        
        #region Internal Level

        private void HandleUpdateGameLevel(int currentLevel)
        {
            
        }

        #endregion

        #region Score
        
        private int GetCurrentPoints()
        {
            return _storedPoints;
        }

        private int GetPointsMultiplier()
        {
            return GameConfigManager.Instance.GetGameplayData().PointsMultiplier;
        }

        
        private void Localization_OnLanguageChangedHandler()
        {
            var lastText = _scoreTextValue;
            _scoreTextValue = GetLocalizedString("Gameplay.Score");
        }
        
        #endregion
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
    }
}