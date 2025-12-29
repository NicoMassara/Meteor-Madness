using MeteorMadness.Contracts.Events;
using MeteorMadness.GlobalValues.Events;
using NicolasMassara.CustomTimerManager;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace MeteorMadness.Services.AdsSystem.Controllers
{
    public class AdRewardedController : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_IOS
        [Header("Delay")]
        [Range(0,5)]
        [SerializeField] private float openDelay = 0.5f;
        [Range(0,5)]
        [SerializeField] private float finishDelay = 0.5f;

        private int _retryCount = 0;
        private const int MaxRetryCount = 5;
        
        private bool _failedToLoad = false;
        private bool _hasRewarded = false;
        
        
        protected virtual void Start()
        {
            AdsEvents.Rewarded_OnLoad += TryLoadAd;
            AdsEvents.Rewarded_OnShow += TryShowAd;
        }

        protected void TryLoadAd()
        {
            if(AdsTools.GetAreAdsDisable()) return;
            
            _hasRewarded = false;

            if (AdManager.RewardedAd == null)
            {
                Debug.LogWarning("Ad Manager - Rewarded Ad is null");
                return;
            }

            AdManager.RewardedAd.TryLoad(
                onLoaded: OnLoaded,
                onFailed: OnFailed
            );
        }
        
        protected void TryShowAd()
        {
            if(AdsTools.GetAreAdsDisable()) return;
            
            if (AdManager.RewardedAd == null)
            {
                Debug.LogWarning("Ad Manager - Rewarded Ad is null");
                return;
            }
            
            TimerManager.Add(new TimerData(openDelay, () =>
            {
                if (_failedToLoad)
                {
                    Reward();
                    _failedToLoad = false;
                }
                else
                {
                    AdManager.RewardedAd.TryShow(
                        onDisplayed: OnDisplayed,
                        onDisplayFailed: OnDisplayedFailed,
                        onRewarded: OnRewarded,
                        onClicked: OnClicked,
                        onClose: OnClose,
                        onInfoChanged: OnInfoChanged
                    );
                }
                
            }));
        }

        protected void Reward()
        {
            if(AdsTools.GetAreAdsDisable()) return;
            
            if(_hasRewarded) return;
            
            _hasRewarded = true;
            
            TimerManager.Add(new TimerData(finishDelay, AdsEvents.Rewarded_TriggerReward));
        }

        #region Load

        protected virtual void OnLoaded(LevelPlayAdInfo adInfo)
        {
            _retryCount = 0;
            Debug.Log("Rewarded Ad Loaded");
        }
        
        protected virtual void OnFailed(LevelPlayAdError adError)
        {
            Debug.Log("Rewarded Ad Load Failed, retrying");
            
            if (_retryCount >= MaxRetryCount)
            {
                _retryCount = 0;
                _failedToLoad = true;
            }
            
            TryLoadAd();
            _retryCount++;
        }

        #endregion

        #region Show

        protected virtual void OnDisplayed(LevelPlayAdInfo adInfo)
        {
            _retryCount = 0;
        }
        
        protected virtual void OnDisplayedFailed(LevelPlayAdInfo adInfo, LevelPlayAdError adError)
        {
            Debug.Log("Rewarded Ad Display Failed, retrying");
            
            if (_retryCount >= MaxRetryCount)
            {
                _retryCount = 0;
                Reward();
                return;
            }
            
            TryLoadAd();
            _retryCount++;
        }
        
        protected virtual void OnRewarded(LevelPlayAdInfo adInfo, LevelPlayReward adReward)
        {
            Reward();
        }
        
        protected virtual void OnClicked(LevelPlayAdInfo adInfo)
        {

        }
        
        protected virtual void OnClose(LevelPlayAdInfo adInfo)
        {
            Reward();
        }
        
        protected virtual void OnInfoChanged(LevelPlayAdInfo adInfo)
        {

        }

        #endregion

#endif
    }
}