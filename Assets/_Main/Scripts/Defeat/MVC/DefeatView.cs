using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.GameConfig;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.Interfaces.Analytics;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.SecurityData;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    public class DefeatView : MonoBehaviour, IObserver,
        DefeatView.IDefeatView,
        IDefeatAnalytics
    {
        public interface IDefeatView
        {
            public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
            public event Action OnDataInitialized;
            public event Action OnGameSaved;
            
            public event Action<bool, uint, uint> OnNewCoinsAdded;
        }
        
        #region IDefeatView

        public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
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
                    HandleInitializeData((GeneratedId)args[0],(GeneratedId)args[1],(bool)args[2]);
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
            var storedCoins = GameManager.Instance.GetStoredCoins();
            uint gainedCoins = 0;

            if (SecureValueManager.GetDoesContainValue(GameManager.Instance.StatsController.GetCurrentScoreSecuredId(),
                    out uint currentScore))
            {
                if (GameManager.Instance.TryAddCoins(currentScore))
                {
                    gainedCoins = GameManager.Instance.GetStoredCoins() - storedCoins;
                }
            }
            
            Debug.Log($"Score: {currentScore}, Last Coins: {storedCoins}, Current Coins: {GameManager.Instance.GetStoredCoins()}");
            
            var hasGainedCoins = gainedCoins > 0;
            
            OnNewCoinsAdded?.Invoke(hasGainedCoins, storedCoins, gainedCoins);
        }

        #endregion

        private void HandleInitializeData(GeneratedId highScoreId, GeneratedId currentScoreId, bool hasNewHighScore)
        {
            if (hasNewHighScore)
            {
                GameManager.Instance.StatsController.SaveRuntimeHighScore(highScoreId, currentScoreId);
            }
            
            AdsEvents.Rewarded_TriggerLoad();
            OnDataInitialized?.Invoke();
        }

        private void SaveGameData()
        {
            GameManager.Instance.StatsController.SaveHighScore(GameManager.Instance.StatsController.GetHighScoreSecuredId());
            GameManager.Instance.StatsController.SaveStats();
            GameManager.Instance.SaveCoins();
 
        }
        
        private void HandleExecuteDisable()
        {
            GameManager.Instance.StatsController.ClearScoreData();
            GameScreenEventCaller.DisableScreen(ScreenType.Defeat, EventRequestType.Granted);
        }
        
        private void HandleDataLoaded()
        {
            OnDataLoaded?.Invoke(
                GameManager.Instance.StatsController.GetCurrentScoreSecuredId(), 
                GameManager.Instance.StatsController.GetHighScoreSecuredId(),
                GameManager.Instance.StatsController.GetHasNewHighScore());

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