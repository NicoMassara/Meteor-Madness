using System;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
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
            if (FlagsManager.GetHasOpenedStats() == false)
            {
                FlagsManager.SetHasOpenedStats();
                FlagsManager.SaveFlags();
                
                OnFirstOpen?.Invoke();
            }
        }

        private void HandleMainMenu()
        {
            GameManager.Instance.LoadMainMenu();
        }

        private void HandleInitialize()
        {
            OnInitialize?.Invoke(new StatsData
            {
                DeflectAmount = (uint)StatsManager.GetValueByStat(StatType.Deflect),
                CollisionAmount = (uint)StatsManager.GetValueByStat(StatType.Collision),
                AbilityUseAmount = (uint)StatsManager.GetValueByStat(StatType.Ability),
                GamesPlayed = (uint)StatsManager.GetValueByStat(StatType.TimesPlayed),
                DeflectStreak = (uint)StatsManager.GetValueByStat(StatType.Streak),
                HighScore = (uint)StatsManager.GetValueByStat(StatType.HighScore),
                TotalScore = (uint)StatsManager.GetValueByStat(StatType.TotalScored),
                LongestTime = (uint)StatsManager.GetValueByStat(StatType.LongestTime)
            });
        }
        
        private void HandleEnable()
        {

        }
        
        private void HandleExecuteDisable()
        {

            GameScreenEventCaller.DisableScreen(ScreenType.Stats, EventRequestType.Granted);
        }
    }
}