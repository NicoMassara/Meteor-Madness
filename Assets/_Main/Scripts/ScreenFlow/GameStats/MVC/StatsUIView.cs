using System;
using _Main.Scripts.GameStats;
using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers.GameConfig;
using MeteorMadness.ScreenFlow.Base;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Stats
{
    public class StatsUIView : BaseViewUI<StatsUiSelector,StatsUiComponents>, IObserver,
        StatsUIView.IStatsUIView, IStatsUIVibration
    {
        public interface IStatsUIView
        {
            public event Action OnBackButtonPressed;
            public event Action OnTextsLoaded;
        }
        
        public event Action OnBackButtonPressed;
        public event Action OnTextsLoaded;

        #region IStatsUIVibration

        public event Action OnCloseFirstButtonPressed;

        #endregion

        private void Start()
        {
            UIComponents.SetActiveFirstOpenPanel(false);
        }

        private void EnableButtons()
        {
            UIComponents.AddBackButtonListener(OnBackButtonPressedHandler);
        }

        private void DisableButtons()
        {
            UIComponents.RemoveBackButtonListener(OnBackButtonPressedHandler);
        }

        public override void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // === Enable / Disable === //
                case StatsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case StatsObserverMessage.StartDisable:
                    HandleStartDisable();
                   break;
                
                // === Text Data === ///
                
                case StatsObserverMessage.LoadTextData:
                    HandleLoadTextData((StatsData)args[0]);
                    break;
                
                // === First Open === //
                case StatsObserverMessage.FirstOpen:
                    HandleFirstOpen();
                    break;
            }
        }

        #region First Open

        private void HandleFirstOpen()
        {
            UIComponents.SetActiveFirstOpenPanel(true);
            UIComponents.SetInteractableFirstOpenCloseButton(false);
            UIComponents.AddListenerToFirstOpenCloseButton(OnFirstOpenPanelClosed);
            TimerManager.Add(new TimerData(1.5f, 
                () => UIComponents.SetInteractableFirstOpenCloseButton(true)));
        }

        private void OnFirstOpenPanelClosed()
        {
            OnCloseFirstButtonPressed?.Invoke();
            UIComponents.RemoveListenerToFirstOpenCloseButton(OnFirstOpenPanelClosed);
            UIComponents.SetActiveFirstOpenPanel(false);
        }

        #endregion

        #region Text Data 

        private void HandleLoadTextData(StatsData statsData)
        {
            UIComponents.SetDeflectAmountText("Stats.Deflect", statsData.DeflectAmount);
            UIComponents.SetCollisionAmountText("Stats.Collision", statsData.CollisionAmount);
            UIComponents.SetAbilityUseAmountText("Stats.AbilityCount", statsData.AbilityUseAmount);
            UIComponents.SetGamesPlayedText("Stats.GamesPlayed", statsData.GamesPlayed);
            UIComponents.SetDeflectStreakText("Stats.DeflectStreak",statsData.DeflectStreak);
            UIComponents.SetLongestTimeText("Stats.LongestTime",statsData.LongestTime);
            UIComponents.SetHighScoreText("Stats.HighScore", statsData.HighScore);
            

            var finalScore = (statsData.TotalScore * GetPointsMultiplier());
            Debug.Log($"High Score Data: {statsData.HighScore}, Deflect Streak: {statsData.DeflectStreak}, Longest Time: {statsData.LongestTime}, Total Score: {finalScore}");
            UIComponents.SetAllScoreText("Stats.HistoricScore",finalScore);
            
            OnTextsLoaded?.Invoke();
        }
        #endregion

        #region Enable / Disable
        
        private void HandleEnable()
        {
            EnableButtons();
        }
        
        private void HandleStartDisable()
        {
            DisableButtons();
        }
        
        private void OnBackButtonPressedHandler()
        {
            OnBackButtonPressed?.Invoke();
        }
        
        #endregion

        #region Tools

        private int GetPointsMultiplier() 
            => GameConfigManager.Instance.GetGameplayData().PointsMultiplier;

        #endregion
    }
}