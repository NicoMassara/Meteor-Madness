using System;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Save;
using UnityEngine;

namespace _Main.Scripts.GameStats
{
    public class StatsView : MonoBehaviour, StatsView.IStatsView, IObserver
    {
        public interface IStatsView
        {
            public event Action<StatsData> OnInitialize;
        }
        
        #region IStatsView
        
        public event Action<StatsData> OnInitialize;
        
        #endregion

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case StatsObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                case StatsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case StatsObserverMessage.ExecuteDisable:
                    HandleExecuteDisable();
                    break;
                case StatsObserverMessage.MainMenu:
                    HandleMainMenu();
                    break;
            }
        }

        private void HandleMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }

        private void HandleInitialize()
        {
            GameManager.Instance.FlagsController.GetHasOpenedStats();
            
            var dataManager = DataManager.Instance;
            var saveData = dataManager.GetData<DataManager.StatsSaveData>(DataManager.SaveDataType.Stats);
            
            OnInitialize?.Invoke(new StatsData
            {
                DeflectAmount = saveData.DeflectAmount,
                CollisionAmount = saveData.CollisionAmount,
                AbilityUseAmount = saveData.AbilityUseAmount,
                GamesPlayed = saveData.GamesPlayed,
                DeflectStreak = saveData.LongestStreak,
                LongestTime = saveData.LongestTime,
                HighScore = saveData.HighScore,
                TotalScore = saveData.TotalScore,
            });
        }
        
        private void HandleEnable()
        {
            CameraEventCaller.LookRight();
        }
        
        private void HandleExecuteDisable()
        {
            CameraEventCaller.LookCenter();
            GameScreenEventCaller.DisableScreen(ScreenType.Stats, EventRequestType.Granted);
        }
    }
}