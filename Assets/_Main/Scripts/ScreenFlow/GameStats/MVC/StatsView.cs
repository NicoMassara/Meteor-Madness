using System;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Save;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Stats
{
    public class StatsView : MonoBehaviour, StatsView.IStatsView, IObserver
    {
        public interface IStatsView
        {
            public event Action<StatsData> OnInitialize;
            public event Action OnFirstOpen;
        }
        
        #region IStatsView
        public event Action OnFirstOpen;
        public event Action<StatsData> OnInitialize;
        
        #endregion

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // === Enable / Disable === //
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
                
                // === Opened === //
                case StatsObserverMessage.Opened:
                    HandleOpened();
                    break;
            }
        }

        private void HandleOpened()
        {
            var hasOpened = GameManager.Instance.FlagsController.GetHasOpenedStats();
            
            if (hasOpened == false)
            {
                OnFirstOpen?.Invoke();
            }
        }

        private void HandleMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }

        private void HandleInitialize()
        {
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