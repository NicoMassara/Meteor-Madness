using _Main.Scripts.Interfaces.Ads;
using NicolasMassara.CustomTimerManager;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace _Main.Scripts.AdsSystem.AdsComponents.Base
{
    public abstract class BaseBannerComponent <T> : MonoBehaviour
        where T : IBannerComponent
    {
#if UNITY_ANDROID || UNITY_IOS
        [Header("Delay")]
        [Range(0,5)]
        [SerializeField] private float openDelay = 0.5f;
        [Range(0,5)]
        [SerializeField] private float hideDelay = 0.5f;   
        
        protected T ComponentToBanner { get;  private set; }

        private int _retryCount = 0;
        private const int MaxRetryCount = 5;
        
        private void Awake()
        {
            ComponentToBanner = GetComponent<T>();
        }
        
        protected virtual void Start()
        {
            ComponentToBanner.OnLoadAd += TryLoadAd;
            ComponentToBanner.OnShowAd += TryShowAd;
            ComponentToBanner.OnHideAd += TryHideAd;
            ComponentToBanner.OnDestroyAd += TryDestroyAd;
        }
        
        private void TryLoadAd()
        {
            if (AdManager.BannerAd == null)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is null");
                return;
            }
            
            AdManager.RewardedAd.TryLoad(
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
            if (AdManager.BannerAd == null)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is null");
                return;
            }

            if (AdManager.BannerAd.IsDisplaying)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is already displaying");
                return;
            }

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
            if (AdManager.BannerAd == null)
            {
                Debug.LogWarning("Ad Manager - Banner Ad is null");
                return;
            }

            TimerManager.Add(new TimerData(hideDelay, ()=> AdManager.BannerAd.TryHide()));
        }

        private void TryDestroyAd()
        {
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