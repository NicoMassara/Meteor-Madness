using System;
using MeteorMadness.GlobalEvents.Events;
using MeteorMadness.GlobalValues.BaseSingleton;
using MeteorMadness.Services.AdsSystem.Controllers;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace MeteorMadness.Services.AdsSystem
{
    [RequireComponent(typeof(AdBannerController))]
    [RequireComponent(typeof(AdRewardedController))]
    public class AdManager : SingletonBehaviour<AdManager>
    {
#if UNITY_ANDROID || UNITY_IOS
        
        #region Public Interfaces
        
        public interface IInterstitialAd
        {
            public bool IsReady { get; }
            
            public bool TryLoad(
                Action<LevelPlayAdInfo> onLoaded = null,
                Action<LevelPlayAdError> onFailed = null
            );

            public bool TryShow(
                Action<LevelPlayAdInfo> onDisplayed = null,
                Action<LevelPlayAdInfo, LevelPlayAdError> onDisplayFailed = null,
                Action<LevelPlayAdInfo> onClicked = null,
                Action<LevelPlayAdInfo> onClose = null,
                Action<LevelPlayAdInfo> onInfoChanged = null
            );
        }

        public interface IBannerAd
        {
            public bool HasLoaded { get; }
            public bool IsDisplaying { get; }

            public bool TryLoad(
                Action<LevelPlayAdInfo> onLoaded = null,
                Action<LevelPlayAdError> onFailed = null
            );

            public bool TryShow(
                Action<LevelPlayAdInfo> onDisplayed = null,
                Action<LevelPlayAdInfo, LevelPlayAdError> onDisplayFailed = null,
                Action<LevelPlayAdInfo, LevelPlayReward> onRewarded = null,
                Action<LevelPlayAdInfo> onClicked = null,
                Action<LevelPlayAdInfo> onCollapsed = null,
                Action<LevelPlayAdInfo> onLeftApplication = null,
                Action<LevelPlayAdInfo> onExpanded = null
            );

            public bool TryHide();
            public bool TryDestroy();
        }
        
        public interface IRewardedAd
        {
            public bool IsReady { get; }

            public bool TryLoad(
                Action<LevelPlayAdInfo> onLoaded = null,
                Action<LevelPlayAdError> onFailed = null
            );

            public bool TryShow(
                Action<LevelPlayAdInfo> onDisplayed = null,
                Action<LevelPlayAdInfo, LevelPlayAdError> onDisplayFailed = null,
                Action<LevelPlayAdInfo, LevelPlayReward> onRewarded = null,
                Action<LevelPlayAdInfo> onClicked = null,
                Action<LevelPlayAdInfo> onClose = null,
                Action<LevelPlayAdInfo> onInfoChanged = null
            );
        }
        
        #endregion
        
        #region Private Classes

        private class InterstitialAdClass : IInterstitialAd
        {
            private readonly LevelPlayInterstitialAd _interstitialAd;

            // OneShot LOAD
            private Action<LevelPlayAdInfo> _onLoadSuccessOneShot;
            private Action<LevelPlayAdError> _onLoadFailedOneShot;

            // OneShot SHOW
            private Action<LevelPlayAdInfo> _onDisplayedOneShot;
            private Action<LevelPlayAdInfo, LevelPlayAdError> _onDisplayFailedOneShot;
            private Action<LevelPlayAdInfo> _onClickedOneShot;
            private Action<LevelPlayAdInfo> _onCloseOneShot;
            private Action<LevelPlayAdInfo> _onInfoChangedOneShot;

            public bool IsReady => _interstitialAd.IsAdReady();

            public InterstitialAdClass(string adUnitId, LevelPlayInterstitialAd.Config config = null)
            {
                _interstitialAd = new LevelPlayInterstitialAd(adUnitId, config);

                _interstitialAd.OnAdLoaded += OnLoadedEvent;
                _interstitialAd.OnAdLoadFailed += OnLoadFailedEvent;
                _interstitialAd.OnAdDisplayed += OnDisplayedEvent;
                _interstitialAd.OnAdDisplayFailed += OnDisplayFailedEvent;
                _interstitialAd.OnAdClicked += OnClickedEvent;
                _interstitialAd.OnAdClosed += OnClosedEvent;
                _interstitialAd.OnAdInfoChanged += OnInfoChangedEvent;
            }

            #region Handlers

            // === Load ===

            private void OnLoadedEvent(LevelPlayAdInfo adInfo)
            {
                var cb = _onLoadSuccessOneShot;
                ClearLoadOneShot();
                cb?.Invoke(adInfo);
            }

            private void OnLoadFailedEvent(LevelPlayAdError error)
            {
                var cb = _onLoadFailedOneShot;
                ClearLoadOneShot();
                cb?.Invoke(error);
            }

            // === Show ===

            private void OnDisplayedEvent(LevelPlayAdInfo adInfo)
            {
                _onDisplayedOneShot?.Invoke(adInfo);
            }

            private void OnDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
            {
                var cb = _onDisplayFailedOneShot;
                ClearShowOneShot();
                cb?.Invoke(adInfo, error);
            }

            private void OnClickedEvent(LevelPlayAdInfo adInfo)
            {
                _onClickedOneShot?.Invoke(adInfo);
            }

            private void OnClosedEvent(LevelPlayAdInfo adInfo)
            {
                var cb = _onCloseOneShot;
                ClearShowOneShot();
                cb?.Invoke(adInfo);
            }

            private void OnInfoChangedEvent(LevelPlayAdInfo adInfo)
            {
                _onInfoChangedOneShot?.Invoke(adInfo);
            }

            #endregion

            #region Public API

            public bool TryLoad(
                Action<LevelPlayAdInfo> onLoaded = null,
                Action<LevelPlayAdError> onFailed = null)
            {
                ClearLoadOneShot();

                if (IsReady)
                {
                    return false;
                }

                _onLoadSuccessOneShot = onLoaded;
                _onLoadFailedOneShot = onFailed;

                _interstitialAd.LoadAd();
                return true;
            }

            public bool TryShow(
                Action<LevelPlayAdInfo> onDisplayed = null,
                Action<LevelPlayAdInfo, LevelPlayAdError> onDisplayFailed = null,
                Action<LevelPlayAdInfo> onClicked = null,
                Action<LevelPlayAdInfo> onClose = null,
                Action<LevelPlayAdInfo> onInfoChanged = null)
            {
                ClearShowOneShot();

                if (!IsReady)
                {
                    return false;
                }

                _onDisplayedOneShot = onDisplayed;
                _onDisplayFailedOneShot = onDisplayFailed;
                _onClickedOneShot = onClicked;
                _onCloseOneShot = onClose;
                _onInfoChangedOneShot = onInfoChanged;

                _interstitialAd.ShowAd();
                return true;
            }

            #endregion

            #region Cleaners

            private void ClearLoadOneShot()
            {
                _onLoadSuccessOneShot = null;
                _onLoadFailedOneShot = null;
            }

            private void ClearShowOneShot()
            {
                _onDisplayedOneShot = null;
                _onDisplayFailedOneShot = null;
                _onClickedOneShot = null;
                _onCloseOneShot = null;
                _onInfoChangedOneShot = null;
            }

            #endregion
        }
        private class BannerAdClass : IBannerAd
        {
            private readonly LevelPlayBannerAd _bannerAd;
            
            public bool HasLoaded { get; private set; }
            public bool IsDisplaying { get; private set; }
            
            
            // OneShot LOAD
            private Action<LevelPlayAdInfo> _onLoadSuccessOneShot;
            private Action<LevelPlayAdError> _onLoadFailedOneShot;

            // OneShot SHOW
            private Action<LevelPlayAdInfo> _onDisplayedOneShot;
            private Action<LevelPlayAdInfo, LevelPlayAdError> _onDisplayFailedOneShot;
            private Action<LevelPlayAdInfo> _onClickedOneShot;
            private Action<LevelPlayAdInfo> _onCollapsedOneShot;
            private Action<LevelPlayAdInfo> _onLeftApplicationOneShot;
            private Action<LevelPlayAdInfo> _onExpandedOneShot;
            
            public BannerAdClass(string adUnitId, LevelPlayBannerAd.Config config = null)
            {
                _bannerAd = new LevelPlayBannerAd(adUnitId, config);

                _bannerAd.OnAdLoaded += OnLoadedEvent;
                _bannerAd.OnAdLoadFailed += OnLoadFailedEvent;
                _bannerAd.OnAdDisplayed += OnDisplayedEvent;
                _bannerAd.OnAdDisplayFailed += OnDisplayFailedEvent;
                _bannerAd.OnAdClicked += OnClickedEvent;
                _bannerAd.OnAdCollapsed += OnCollapsedEvent;
                _bannerAd.OnAdLeftApplication += OnLeftApplicationEvent;
                _bannerAd.OnAdExpanded += OnExpandedEvent;
            }
            
            #region Handlers

            // === Load ===

            private void OnLoadedEvent(LevelPlayAdInfo adInfo)
            {
                var cb = _onLoadSuccessOneShot;
                ClearLoadOneShot();
                HasLoaded = true;
                cb?.Invoke(adInfo);
            }

            private void OnLoadFailedEvent(LevelPlayAdError error)
            {
                var cb = _onLoadFailedOneShot;
                ClearLoadOneShot();
                cb?.Invoke(error);
            }

            // === Show ===

            private void OnDisplayedEvent(LevelPlayAdInfo adInfo)
            {
                IsDisplaying = true;
                _onDisplayedOneShot?.Invoke(adInfo);
            }

            private void OnDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error)
            {
                var cb = _onDisplayFailedOneShot;
                ClearShowOneShot();
                cb?.Invoke(adInfo, error);
            }

            private void OnClickedEvent(LevelPlayAdInfo adInfo)
            {
                _onClickedOneShot?.Invoke(adInfo);
            }

            private void OnCollapsedEvent(LevelPlayAdInfo adInfo)
            {
                _onCollapsedOneShot?.Invoke(adInfo);
            }

            private void OnLeftApplicationEvent(LevelPlayAdInfo adInfo)
            {
                _onLeftApplicationOneShot?.Invoke(adInfo);
            }

            private void OnExpandedEvent(LevelPlayAdInfo error)
            {
                _onExpandedOneShot?.Invoke(error);
            }

            #endregion

            #region Public API

            public bool TryLoad(
                Action<LevelPlayAdInfo> onLoaded = null,
                Action<LevelPlayAdError> onFailed = null)
            {
                if (HasLoaded)
                {
                    return false;
                }

                ClearLoadOneShot();

                _onLoadSuccessOneShot = onLoaded;
                _onLoadFailedOneShot = onFailed;

                _bannerAd.LoadAd();
                return true;
            }

            public bool TryShow(
                Action<LevelPlayAdInfo> onDisplayed = null,
                Action<LevelPlayAdInfo, LevelPlayAdError> onDisplayFailed = null,
                Action<LevelPlayAdInfo, LevelPlayReward> onRewarded = null,
                Action<LevelPlayAdInfo> onClicked = null,
                Action<LevelPlayAdInfo> onCollapsed = null,
                Action<LevelPlayAdInfo> onLeftApplication = null,
                Action<LevelPlayAdInfo> onExpanded = null
            )
            {
                if (IsDisplaying)
                {
                    return false;
                }

                ClearShowOneShot();

                _onDisplayedOneShot = onDisplayed;
                _onDisplayFailedOneShot = onDisplayFailed;
                _onClickedOneShot = onClicked;
                _onCollapsedOneShot = onCollapsed;
                _onLeftApplicationOneShot = onLeftApplication;
                _onExpandedOneShot = onExpanded;
                
                _bannerAd.ShowAd();
                return true;
            }

            public bool TryHide()
            {
                IsDisplaying = false;
                _bannerAd.HideAd();
                ClearShowOneShot();
                return true;
            }

            public bool TryDestroy()
            {
                if (HasLoaded == false)
                {
                    return false;
                }

                HasLoaded = false;
                _bannerAd.DestroyAd();
                return false;
            }

            #endregion
            
            #region Cleaners

            private void ClearLoadOneShot()
            {
                _onLoadSuccessOneShot = null;
                _onLoadFailedOneShot = null;
            }

            private void ClearShowOneShot()
            {
                _onDisplayedOneShot = null;
                _onDisplayFailedOneShot = null;
                _onClickedOneShot = null;
                _onCollapsedOneShot = null;
                _onLeftApplicationOneShot = null;
                _onExpandedOneShot = null;
            }

            #endregion
        }
        private class RewardedAdClass : IRewardedAd
        {
            private readonly LevelPlayRewardedAd _rewardedVideoAd;

            // OneShot LOAD
            private Action<LevelPlayAdInfo> _onLoadSuccessOneShot;
            private Action<LevelPlayAdError> _onLoadFailedOneShot;

            // OneShot SHOW
            private Action<LevelPlayAdInfo> _onDisplayedOneShot;
            private Action<LevelPlayAdInfo,LevelPlayAdError> _onDisplayFailedOneShot;
            private Action<LevelPlayAdInfo,LevelPlayReward> _onRewardedOneShot;
            private Action<LevelPlayAdInfo> _onClickedOneShot;
            private Action<LevelPlayAdInfo> _onCloseOneShot;
            private Action<LevelPlayAdInfo> _onInfoChangedOneShot;
            
            public bool IsReady => _rewardedVideoAd.IsAdReady();
            
            
            public RewardedAdClass(string adUnitId, LevelPlayRewardedAd.Config config = null)
            {
                _rewardedVideoAd = new LevelPlayRewardedAd(adUnitId,config);
                
                _rewardedVideoAd.OnAdLoaded += OnLoadedEvent;
                _rewardedVideoAd.OnAdLoadFailed += OnLoadFailedEvent;
                _rewardedVideoAd.OnAdDisplayed += OnDisplayedEvent;
                _rewardedVideoAd.OnAdDisplayFailed += OnDisplayedFailedEvent;
                _rewardedVideoAd.OnAdRewarded += OnRewardedEvent;
                _rewardedVideoAd.OnAdClicked += OnClickedEvent;
                _rewardedVideoAd.OnAdClosed += OnClosedEvent;
                _rewardedVideoAd.OnAdInfoChanged += OnInfoChangedEvent;
            }

            #region Handlers
            
            // === Load === ///
            
            private void OnLoadedEvent(LevelPlayAdInfo input) => _onLoadSuccessOneShot?.Invoke(input);

            private void OnLoadFailedEvent(LevelPlayAdError input) => _onLoadFailedOneShot?.Invoke(input);

            // === Show === ///
            private void OnDisplayedEvent(LevelPlayAdInfo input) => _onDisplayedOneShot?.Invoke(input);

            private void OnDisplayedFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError adError) => _onDisplayFailedOneShot?.Invoke(adInfo, adError);

            private void OnRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward reward) => _onRewardedOneShot?.Invoke(adInfo, reward);

            private void OnClickedEvent(LevelPlayAdInfo input) => _onClickedOneShot?.Invoke(input);

            private void OnClosedEvent(LevelPlayAdInfo input) => _onCloseOneShot?.Invoke(input);

            private void OnInfoChangedEvent(LevelPlayAdInfo input) => _onInfoChangedOneShot?.Invoke(input);

            #endregion

            #region Loaders

            public bool TryLoad(
                Action<LevelPlayAdInfo> onLoaded = null, 
                Action<LevelPlayAdError> onFailed = null)
            {
                ClearLoadOneShot();
                
                if (IsReady)
                {
                    return false;
                }
                
                _onLoadSuccessOneShot = onLoaded;
                _onLoadFailedOneShot = onFailed;
                
                _rewardedVideoAd.LoadAd();
                
                return true;
            }

            public bool TryShow(
                Action<LevelPlayAdInfo> onDisplayed = null, 
                Action<LevelPlayAdInfo,LevelPlayAdError> onDisplayFailed = null, 
                Action<LevelPlayAdInfo, LevelPlayReward> onRewarded = null,
                Action<LevelPlayAdInfo> onClicked = null, 
                Action<LevelPlayAdInfo> onClose = null, 
                Action<LevelPlayAdInfo> onInfoChanged = null)
            {
                ClearShowOneShot();

                if (IsReady == false)
                {
                    return false;
                }
                
                _onDisplayedOneShot = onDisplayed;
                _onDisplayFailedOneShot = onDisplayFailed;
                _onRewardedOneShot = onRewarded;
                _onClickedOneShot = onClicked;
                _onCloseOneShot = onClose;
                _onInfoChangedOneShot = onInfoChanged;
                
                
                _rewardedVideoAd.ShowAd();
                
                return true;
            }
            
            
            #endregion

            #region Cleaners

            private void ClearLoadOneShot()
            {
                _onLoadSuccessOneShot = null;
                _onLoadFailedOneShot = null;
            }
            
            private void ClearShowOneShot()
            {
                _onDisplayedOneShot = null;
                _onDisplayFailedOneShot = null;
                _onRewardedOneShot = null;
                _onClickedOneShot = null;
                _onCloseOneShot = null;
                _onInfoChangedOneShot = null;
            }

            #endregion
        }


        #endregion

        // ======================================= //
        
        private IRewardedAd _rewardedAd;
        private IBannerAd _bannerAd;
        private IInterstitialAd _interstitialAd;
        public static IRewardedAd RewardedAd => Instance._rewardedAd;
        public static IBannerAd BannerAd => Instance._bannerAd;
        public static IInterstitialAd InterstitialAd => Instance._interstitialAd;
        
        private void Awake()
        {
            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }

        private void Initialize()
        {
            BootEvents.OnMainSystemRequestInitialize -= Initialize;
            //

            _rewardedAd = new RewardedAdClass(AdConfig.RewardedVideoAdUnitId,GetRewardedConfig());
            _bannerAd = new BannerAdClass(AdConfig.BannerAdUnitId,GetBannerConfig());
            _interstitialAd = new InterstitialAdClass(AdConfig.InterstitalAdUnitId,GetInterstitialConfig());
            
            //
            BootEvents.MainSystemInitialized();
        }

        private LevelPlayRewardedAd.Config GetRewardedConfig()
        {
            return new LevelPlayRewardedAd.Config.Builder()
                .Build();
        }
        
        private LevelPlayBannerAd.Config GetBannerConfig()
        {
            return new LevelPlayBannerAd.Config.Builder()
                .SetPosition(LevelPlayBannerPosition.BottomCenter)
                .SetSize(LevelPlayAdSize.BANNER)
                .SetRespectSafeArea(true)
                .Build();
        }
        
        private LevelPlayInterstitialAd.Config GetInterstitialConfig()
        {
            return new LevelPlayInterstitialAd.Config.Builder()
                .Build();
        }

        public void PauseGame(bool pause)
        {
            LevelPlay.SetPauseGame(pause);
        }

#endif
    }
}