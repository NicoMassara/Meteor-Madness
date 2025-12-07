using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.ViewUI;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIView : BaseViewUI<GameModeUiPanelSelector,GameModeUIComponents>,IUpdatable, IObserver, IGameModeUISounds,
        GameModeUIView.IGameModeUIView
    {
        public interface IGameModeUIView
        {
            public event Action OnPauseButtonPressed;
            public event Action OnFinishAddingPoints;
        }
        
        private NumberIncrementer _numberIncrementer;
        private bool _hasHighScore;
        private float _highScore;
        private string _scoreTextValue;
        private uint _storedPoints = 0;
        
        public event Action OnPauseButtonPressed;
        public event Action OnFinishAddingPoints;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Awake()
        {
            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            //
            Localization_OnLanguageChangedHandler();
            LocalizationEvents.OnLanguageChanged += Localization_OnLanguageChangedHandler;
            
            _numberIncrementer = new NumberIncrementer(HandleUpdatePointsText,OnFinishAddingPoints);
            
            BootEvents.SubSystemInitialized();
        }
        
        public void ExecuteUpdate(float deltaTime)
        {
            if (_numberIncrementer != null)
            {
                if (!_numberIncrementer.IsFinished)
                {
                    _numberIncrementer.Run(deltaTime);
                }
            }
        }

        public override void OnNotify(ulong message, params object[] args)
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
                
                //=== Meteor ===//
                case GameModeObserverMessage.PointsGained:
                    HandlePointsGained((uint)args[1]);
                    break;
                
                //=== Disable ===//
                case GameModeObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                
                //=== Data ===/
                case GameModeObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
            }
        }
        

        #region Data

        private void HandleInitializeData()
        {
            _numberIncrementer.ResetValues();
            _storedPoints = 0;
            UpdateScoreTextLocalization();
        }
        
        private void HandleStartDisable()
        {
            _storedPoints = 0;
        }

        #endregion
        
        #region Score
        
        private void HandlePointsGained(uint points)
        {
            _numberIncrementer.AddValue(points * (uint)GetPointsMultiplier());
        }
        
        private uint GetCurrentIncrementerPoints()
        {
            return (uint)_numberIncrementer.CurrentValue;
        }
        
        private void HandleUpdatePointsText(uint amount)
        {
            UIComponents.ScoreText.text = $"{_scoreTextValue}:{amount:D6}";;

            SetStoredPoints(amount);
        }

        private void UpdateScoreTextLocalization()
        {
            var textValue = $"{_scoreTextValue}:{_storedPoints:D6}";
            
            UIComponents.ScoreText.text = textValue;
        }

        private void SetStoredPoints(uint storedPoints)
        {
            _storedPoints = storedPoints;
            GameManager.Instance.VisualPoints = _storedPoints;
        }
        

        private int GetPointsMultiplier()
        {
            return GameConfigManager.Instance.GetGameplayData().PointsMultiplier;
        }
        
        private void Localization_OnLanguageChangedHandler()
        {
            _scoreTextValue = GetLocalizedString("Gameplay.Score");
            UpdateScoreTextLocalization();
        }
        
        #endregion
        
        #region Countdown

        private void HandleUpdateCountdown(float countdownTime)
        {
            var text = countdownTime >= 1 ? $"{GetLocalizedString("Gameplay.CountDownStart")} {(int)countdownTime}..." 
                : GetLocalizedString("Gameplay.CountdownFinish");
            UIComponents.CountdownText.text = text;
        }

        #endregion

        #region Gameplay UI

        private void HandleEnableGameplayUI()
        {
            UIComponents.PauseButton.onClick.AddListener(() =>
            {
                OnPauseButtonPressed?.Invoke();
            });
        }
        
        private void HandleDisableGameplayUI()
        {
            UIComponents.PauseButton.onClick.RemoveAllListeners();
        }

        #endregion
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
    }
}