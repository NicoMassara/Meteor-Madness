using System;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.Managers.Localization;
using MeteorMadness.ScreenFlow.Base;
using NicolasMassara.CustomUpdateManager;

namespace MeteorMadness.ScreenFlow.GameMode
{
    public class GameModeUIView : BaseViewUI<GameModeUiPanelSelector,GameModeUIComponents>,IUpdatable, IObserver, IGameModeUISounds,
        GameModeUIView.IGameModeUIView, IGameModeUIVibration
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
        private string _streakTextValue;
        private uint _storedStreak = 0;

        #region IGameModeUIVibration

        public event Action OnPointsAdded;

        #endregion
        
        public event Action OnPauseButtonPressed;
        public event Action OnFinishAddingPoints;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.UI;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        private void Awake()
        {
            UIComponents.CountdownText.text = "";
            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            //
            Localization_OnLanguageChangedHandler();
            LocalizationEvents.OnLanguageChanged += Localization_OnLanguageChangedHandler;
            
            _numberIncrementer = new NumberIncrementer(HandleUpdatePointsText,OnPointsAdded,OnFinishAddingPoints);
            
            BootEvents.SubSystemInitialized();
        }

        private void OnDestroy()
        {
            LocalizationEvents.OnLanguageChanged -= Localization_OnLanguageChangedHandler;
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
                case GameModeObserverMessage.FinishCountdown:
                    HandleFinishCountdown();
                    break;
                
                //=== Meteor ===//
                case GameModeObserverMessage.PointsGained:
                    HandlePointsGained((uint)args[1]);
                    break;
                
                //=== Disable ===//
                case GameModeObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                
                //=== Data ===//
                case GameModeObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
                
                // === Streak === //
                case GameModeObserverMessage.UpdateStreak:
                    HandleUpdateStreak((uint)args[0]);
                    break;
                case GameModeObserverMessage.NotifyStreak:
                    HandleNotifyStreak((int)args[0]);
                    break;
            }
        }
        

        #region Data

        private void HandleInitializeData()
        {
            GameManager.Instance.VisualPoints = 0;
            
            _numberIncrementer.ResetValues();
            _storedPoints = 0;
            UpdateScoreTextLocalization();

            _storedStreak = 0;
            UpdateStreakTextLocalization();
            
        }
        
        private void HandleStartDisable()
        {
            _storedPoints = 0;
            _storedStreak = 0;
        }
        
        #endregion

        #region Streak
        
        private void HandleUpdateStreak(uint amount)
        {
            UIComponents.SetStreakText(_streakTextValue, amount);
            
            _storedStreak = amount;
        }
        
        private void HandleNotifyStreak(int streakAmount)
        {
            string localizedText = GetLocalizedString("Gameplay.StreakNotify");
            string result = localizedText.Replace("%%", $"{streakAmount}");
            
            UIComponents.SetNotifyText(result);
        }

        private void UpdateStreakTextLocalization()
        {
            var textValue = $"{_streakTextValue}: {_storedStreak:D4}";
            
            UIComponents.StreakText.text = textValue;
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
            UIComponents.SetScoreText(_scoreTextValue,amount);

            SetStoredPoints(amount);
        }

        private void UpdateScoreTextLocalization()
        {
            UIComponents.SetScoreText(_scoreTextValue,_storedPoints);
        }

        private void SetStoredPoints(uint storedPoints)
        {
            _storedPoints = storedPoints;
            GameManager.Instance.VisualPoints = (uint)(_storedPoints * GetPointsMultiplier());
        }
        

        private int GetPointsMultiplier()
        {
            return GameConfigManager.Instance.GetGameplayData().PointsMultiplier;
        }
        
        private void Localization_OnLanguageChangedHandler()
        {
            _scoreTextValue = GetLocalizedString("Gameplay.Score");
            _streakTextValue = GetLocalizedString("Gameplay.Streak");
            UpdateScoreTextLocalization();
            UpdateStreakTextLocalization();
        }
        
        #endregion
        
        #region Countdown

        private void HandleUpdateCountdown(float countdownTime) => UIComponents.SetCountdownText(countdownTime);

        private void HandleFinishCountdown() => UIComponents.SetCountdownText(0f);

        #endregion

        #region Gameplay UI

        private void TriggerPauseButtonPressed() => OnPauseButtonPressed?.Invoke();

        private void HandleEnableGameplayUI() 
            => UIComponents.AddListenerToPauseButton(TriggerPauseButtonPressed);

        private void HandleDisableGameplayUI() 
            => UIComponents.RemoveListenerToPauseButton(TriggerPauseButtonPressed);

        #endregion
        
        private string GetLocalizedString(string key) 
            => LocalizationManager.Instance.GetText(key);
    }
}