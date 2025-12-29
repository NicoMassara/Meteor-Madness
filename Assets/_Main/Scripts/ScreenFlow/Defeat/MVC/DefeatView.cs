using System;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces.Analytics;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.Managers;
using MeteorMadness.Managers.Cosmetics;
using UnityEngine;

namespace MeteorMadness.ScreenFlow.Defeat
{
    public class DefeatView : MonoBehaviour, IObserver,
        DefeatView.IDefeatView,
        IDefeatAnalytics
    {
        public interface IDefeatView
        {
            public event Action<DefeatScreenData> OnDataLoaded;
            public event Action OnDataInitialized;
            public event Action OnGameSaved;
            
            public event Action<bool, uint, uint> OnNewCoinsAdded;
        }
        
        #region IDefeatView

        public event Action<DefeatScreenData> OnDataLoaded;
        public event Action OnDataInitialized;
        public event Action OnGameSaved;
        public event Action<bool, uint, uint> OnNewCoinsAdded;

        #endregion

        #region IDefeatAnalytics
        public event Action OnRestart;
        public event Action OnMainMenu;

        #endregion

        private void Start()
        {
            AdsEvents.Rewarded_Reward += TriggerReward;
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                // === Disable === //
                case DefeatObserverMessage.ExecuteDisable:
                    HandleExecuteDisable();
                    break;
                
                // === Load Data === //
                case DefeatObserverMessage.LoadData:
                    HandleDataLoaded();
                    break;
                case DefeatObserverMessage.InitializeData:
                    HandleInitializeData();
                    break;
                
                // === Ads === //
                case DefeatObserverMessage.SendAds:
                    HandleSendAds();
                    break;
                
                // === Screens === //
                case DefeatObserverMessage.RestartGame:
                    HandleRestartGame();
                    break;
                case DefeatObserverMessage.LoadMainMenu:
                    HandleLoadMainMenu();
                    break;
                
                // === Coins === //
                case DefeatObserverMessage.CheckNewCoins:
                    HandleCheckNewCoins();
                    break;
            }
        }



        #region Screens
        
        private void HandleLoadMainMenu()
        {
            OnMainMenu?.Invoke();
            GameManager.Instance.LoadMainMenu();
        }

        private void HandleRestartGame()
        {
            OnRestart?.Invoke();
            GameManager.Instance.LoadGameMode();
        }
        
        #endregion

        #region Ads
        
        private void HandleSendAds()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (GameParameters.GameplayValues.AdsEnable)
            {
                AdsEvents.Rewarded_TriggerShow();
            }
            else
            {
                TriggerReward();
            }
#else
            // Skips all Ads system, only for Desktop or Paid Mobile Version
            TriggerReward();
#endif
        }

        #endregion

        #region Coins

        private void HandleCheckNewCoins()
        {
            var storedCoins = SkinManager.Instance.GetCoins();
            uint gainedCoins = 0;

            var currentScore = (uint)StatsManager.GetRuntimeScore();

            if (SkinManager.Instance.TryAddCoins(currentScore))
            {
                gainedCoins = SkinManager.Instance.GetCoins() - storedCoins;
            }
            
            Debug.Log($"Score: {currentScore}, Last Coins: {storedCoins}, Current Coins: {SkinManager.Instance.GetCoins()}");
            
            var hasGainedCoins = gainedCoins > 0;
            
            OnNewCoinsAdded?.Invoke(hasGainedCoins, storedCoins, gainedCoins);
        }

        #endregion

        private void HandleInitializeData()
        {
            AdsEvents.Rewarded_TriggerLoad();
            OnDataInitialized?.Invoke();
        }

        private void SaveGameData()
        {
            StatsManager.SaveValues();
            SkinManager.Instance.SaveStoredCoins();
 
        }
        
        private void HandleExecuteDisable()
        {
            StatsManager.ClearRuntimeData();
            GameScreenEventCaller.DisableScreen(ScreenType.Defeat, EventRequestType.Granted);
        }
        
        private void HandleDataLoaded()
        {
            var runtimeScore = (uint)StatsManager.GetRuntimeScore();
            var storedHighScore = (uint)StatsManager.GetValueByStat(StatType.HighScore);
            var hasNewHighScore = runtimeScore > storedHighScore;
            
            OnDataLoaded?.Invoke(new DefeatScreenData
            {
                Score = runtimeScore,
                HighScore = hasNewHighScore ? runtimeScore : storedHighScore,
                HasNewHighScore = hasNewHighScore
            });
        }
        
        private void TriggerReward()
        {
#pragma warning disable CS0162 // Unreachable code detected
            
            if (GameParameters.GameplayValues.DoesSaveProgress)
            {
                SaveGameData();
            }
            
            OnGameSaved?.Invoke();
#pragma warning restore CS0162 // Unreachable code detected

        }
    }
}