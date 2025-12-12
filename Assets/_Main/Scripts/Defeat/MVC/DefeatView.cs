using System;
using _Main.Scripts.AdsSystem;
using _Main.Scripts.CustomId;
using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace _Main.Scripts.Defeat
{
    public class DefeatView : MonoBehaviour, IObserver,
        DefeatView.IDefeatView
    {
        public interface IDefeatView
        {
            public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
            public event Action OnDataInitialized;
            public event Action OnAdsFinished;
        }

        [SerializeField] private float openAdDelay = 0.5f;
        [SerializeField] private float finishAdDelay = 0.5f;
        
        #region IDefeatView

        public event Action<GeneratedId, GeneratedId, bool> OnDataLoaded;
        public event Action OnDataInitialized;
        public event Action OnAdsFinished;

        #endregion
        
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
                /*case DefeatObserverMessage.SaveHighScore:
                    HandleSaveHighScore();
                    break;*/
                
                //=== Ads ===//
                case DefeatObserverMessage.SendAds:
                    HandleSendAds();
                    break;
            }
        }

        #region Ads
        
        private void HandleSendAds()
        {
            if (AdManager.GetInterstitial() == null)
            {
                OnAdsFinished?.Invoke();
                return;
            }

            TimerManager.Add(new TimerData(openAdDelay, () =>
            {
                AdManager.GetInterstitial().TryLoad(ShowAd, OnAdsFailedLoad);
            }));
        }

        #region Load

        private void ShowAd(string input = "")
        {
            AdManager.GetRewarded().TryShow(
                onShowed: null, 
                onFailed: OnAdsShowFailed, 
                onClicked: null, 
                onCompleted: AdCompleted, 
                onSkipped: OnSkipped);
        }

        private void OnSkipped(string input)
        {
            
        }

        private void OnAdsFailedLoad(string input)
        {
            HandleSendAds();
        }

        #endregion

        #region Show
        
        private void OnAdsShowFailed(string obj)
        {
            ShowAd();
        }
        
        private void AdCompleted(string input)
        {
            HandleSaveHighScore();
            
            TimerManager.Add(new TimerData(finishAdDelay, () =>
            {
                OnAdsFinished?.Invoke();
            }));

        }

        #endregion
        
        #endregion

        private void HandleInitializeData(GeneratedId highScoreId, GeneratedId currentScoreId, bool hasNewHighScore)
        {
            if (hasNewHighScore)
            {
                GameManager.Instance.SaveRuntimeHighScore(highScoreId, currentScoreId);
            }
            
            OnDataInitialized?.Invoke();
        }

        private void HandleSaveHighScore()
        {
            GameManager.Instance.SaveHighScore(GameManager.Instance.GetHighScoreSecuredId());
            GameManager.Instance.SaveStats();
        }
        
        private void HandleExecuteDisable()
        {
            GameScreenEventCaller.DisableScreen(ScreenType.Defeat, EventRequestType.Granted);
        }

        private void HandleStartDisable()
        {
            GameManager.Instance.ClearScoreData();
        }
        
        private void HandleDataLoaded()
        {
            OnDataLoaded?.Invoke(
                GameManager.Instance.CurrentScoreSecuredId, 
                GameManager.Instance.GetHighScoreSecuredId(),
                GameManager.Instance.GetHasNewHighScore());

        }
    }
}