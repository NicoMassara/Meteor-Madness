using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.Interfaces.Analytics;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
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
        }
        
        #region IDefeatView

        public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
        public event Action OnDataInitialized;
        public event Action OnGameSaved;

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
                //=== Disable ===//
                case DefeatObserverMessage.StartDisable:
                    HandleStartDisable();
                    break;
                case DefeatObserverMessage.ExecuteDisable:
                    HandleExecuteDisable();
                    break;
                
                //=== Load Data ===//
                case DefeatObserverMessage.LoadData:
                    HandleDataLoaded();
                    break;
                case DefeatObserverMessage.InitializeData:
                    HandleInitializeData((GeneratedId)args[0],(GeneratedId)args[1],(bool)args[2]);
                    break;
                
                //=== Ads ===//
                case DefeatObserverMessage.SendAds:
                    HandleSendAds();
                    break;
                
                //=== Screens ===//
                case DefeatObserverMessage.RestartGame:
                    HandleRestartGame();
                    break;
                case DefeatObserverMessage.LoadMainMenu:
                    HandleLoadMainMenu();
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
            AdsEvents.Rewarded_TriggerShow();
#else
            // Skips all Ads system, only for Desktop or Paid Mobile Version
            TriggerReward();
#endif
        }

        #endregion

        private void HandleInitializeData(GeneratedId highScoreId, GeneratedId currentScoreId, bool hasNewHighScore)
        {
            if (hasNewHighScore)
            {
                GameManager.Instance.SaveRuntimeHighScore(highScoreId, currentScoreId);
            }
            
            AdsEvents.Rewarded_TriggerLoad();
            OnDataInitialized?.Invoke();
        }

        private void SaveGameData()
        {
            GameManager.Instance.SaveHighScore(GameManager.Instance.GetHighScoreSecuredId());
            GameManager.Instance.SaveStats();
 
        }
        
        private void HandleExecuteDisable()
        {
            GameManager.Instance.ClearScoreData();
            GameScreenEventCaller.DisableScreen(ScreenType.Defeat, EventRequestType.Granted);
        }

        private void HandleStartDisable()
        {

        }
        
        private void HandleDataLoaded()
        {
            OnDataLoaded?.Invoke(
                GameManager.Instance.CurrentScoreSecuredId, 
                GameManager.Instance.GetHighScoreSecuredId(),
                GameManager.Instance.GetHasNewHighScore());

        }
        
        private void TriggerReward()
        {
#pragma warning disable CS0162 // Unreachable code detected
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (GameParameters.GameplayValues.DoesSaveProgress)
            {
                SaveGameData();
            }
#else
            SaveGameData();
#endif
            
            OnGameSaved?.Invoke();
#pragma warning restore CS0162 // Unreachable code detected

        }
    }
}