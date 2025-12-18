using System;
using _Main.Scripts.GameConfig;
using _Main.Scripts.Interfaces.Vibration;
using _Main.Scripts.Observer;
using _Main.Scripts.ViewUI;
using UnityEngine;

namespace _Main.Scripts.GameStats
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
                case StatsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case StatsObserverMessage.StartDisable:
                    HandleStartDisable();
                   break;
                
                case StatsObserverMessage.LoadTextData:
                    HandleLoadTextData((StatsData)args[0]);
                    break;
            }
        }

        private void OnBackButtonPressedHandler()
        {
            OnBackButtonPressed?.Invoke();
        }

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

        private void HandleEnable()
        {
            EnableButtons();
        }
        
        private void HandleStartDisable()
        {
            DisableButtons();
        }
        
        private int GetPointsMultiplier()
        {
            return GameConfigManager.Instance.GetGameplayData().PointsMultiplier;
        }
    }
}