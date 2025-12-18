using _Main.Scripts.GlobalEvents;
using NicolasMassara.CustomTimerManager;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace _Main.Scripts.AdsSystem
{
    public class AdBannerController : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_IOS
        [Header("Delay")]
        [Range(0,5)]
        [SerializeField] private float openDelay = 0.25f;
        [Range(0,5)]
        [SerializeField] private float hideDelay = 0.5f;   

        private int _retryCount = 0;
        private const int MaxRetryCount = 5;

        private TimerManager.GeneratedId _hideId;
        
        protected virtual void Start()
        {
            AdsEvents.Banner_OnLoad += TryLoadAd;
            AdsEvents.Banner_OnShow += TryShowAd;
            AdsEvents.Banner_OnHide += TryHideAd;
            AdsEvents.Banner_OnDestroy += TryDestroyAd;
        }
        
        private void TryLoadAd()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            if(GameParameters.GameplayValues.AdsEnable == false) return;
#endif
            
            if (AdManager.BannerAd == null)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is null");
                return;
            }
            
            AdManager.BannerAd.TryLoad(
                onLoaded: OnLoaded,
                onFailed: OnFailed
            );
        }

        #region Load Handlers
        
        
        protected virtual void OnLoaded(LevelPlayAdInfo adInfo)
        {
            Debug.Log("Ad Manager - Banner Loaded");
        }
        
        protected virtual void OnFailed(LevelPlayAdError adInfo)
        {
            Debug.Log("Rewarded Ad Load Failed, retrying");
            
            if (_retryCount >= MaxRetryCount)
            {
                _retryCount = 0;
                Debug.LogWarning("Rewarded Ad Load Failed, retrying in 30s");
                TimerManager.Add(new TimerData(30, TryLoadAd));
                return;
            }
            
            TryLoadAd();
            _retryCount++;
        }
        
        #endregion

        private void TryShowAd()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            if(GameParameters.GameplayValues.AdsEnable == false) return;
#endif
            
            if (AdManager.BannerAd == null)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is null");
                return;
            }

            // Checks if the Banner is being hidden, if it is, stops it
            
            if (_hideId != null && _hideId.IsActive)
            {
                TimerManager.Remove(_hideId);
                return;
            }
            
            // Checks if banner is already displaying
            
            if (AdManager.BannerAd.IsDisplaying)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is already displaying");
                return;
            }
            
            // Displays banner

            TimerManager.Add(new TimerData(openDelay, () =>
            {
                AdManager.BannerAd.TryShow(
                    onDisplayed: OnDisplayed,
                    onDisplayFailed: OnDisplayedFailed,
                    onClicked: OnClicked,
                    onCollapsed: OnCollapsed,
                    onLeftApplication: OnLeftApplication,
                    onExpanded: OnExpanded
                );
            }));
        }
        
        #region Show Handlers

        protected virtual void OnDisplayed(LevelPlayAdInfo adInfo)
        {
            Debug.Log("Ad Manager - Banner Displayed");
        }

        protected virtual void OnDisplayedFailed(LevelPlayAdInfo adInfo, LevelPlayAdError adError)
        {
            Debug.Log("Rewarded Ad Display Failed, retrying");
            
            if (_retryCount >= MaxRetryCount)
            {
                _retryCount = 0;
                return;
            }
            
            TryLoadAd();
            _retryCount++;
        }

        protected virtual void OnClicked(LevelPlayAdInfo adInfo)
        {

        }

        protected virtual void OnCollapsed(LevelPlayAdInfo adInfo)
        {

        }

        protected virtual void OnLeftApplication(LevelPlayAdInfo adInfo)
        {
  
        }

        protected virtual void OnExpanded(LevelPlayAdInfo adInfo)
        {

        }
        
        #endregion

        private void TryHideAd()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            if(GameParameters.GameplayValues.AdsEnable == false) return;
#endif
            
            if (AdManager.BannerAd == null)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is null");
                return;
            }
            
            if (_hideId != null && _hideId.IsActive)
            {
                return;
            }

            _hideId = TimerManager.Add(new TimerData(hideDelay, ()=> AdManager.BannerAd.TryHide()));
        }

        private void TryDestroyAd()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            if(GameParameters.GameplayValues.AdsEnable == false) return;
#endif
            
            if (AdManager.BannerAd == null)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is null");
                return;
            }
            
            AdManager.BannerAd.TryDestroy();
        }
        
#endif
    }
}