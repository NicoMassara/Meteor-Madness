using System;
using _Main.Scripts.GlobalEvents;
using _Main.Scripts.MyComponents;
using NicolasMassara.CustomTimerManager;
using UnityEngine;
using UnityEngine.Advertisements;

namespace _Main.Scripts.AdsSystem
{
    public class AdManager : SingletonBehaviour<AdManager>
    {
        #region Private Clases
        
        public interface IUnityAd
        {
            public bool IsActive { get; }
            
            void TryLoad(
                Action<string> onLoaded = null,
                Action<string> onFailed = null
            );

            void TryShow(
                Action<string> onShowed = null,
                Action<string> onClicked = null,
                Action<string> onFailed = null,
                Action<string> onCompleted = null,
                Action<string> onSkipped = null
            );
        }
        private class BaseAd : IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAd
        {
            private readonly string _adUnitId;
            private const int TimeOutDelay = 15;
            private TimerManager.GeneratedId _loadTimeOutId;
            private TimerManager.GeneratedId _showTimeOutId;
            
            public bool IsActive { get; private set; }

            // OneShot LOAD
            private Action<string> _onLoadSuccessOneShot;
            private Action<string> _onLoadFailedOneShot;

            // OneShot SHOW
            private Action<string> _onShowSuccessOneShot;
            private Action<string> _onShowClickedOneShot;
            private Action<string> _onShowFailedOneShot;
            private Action<string> _onShowCompletedOneShot;
            private Action<string> _onShowSkippedOneShot;

            public BaseAd(string adUnitId)
            {
                _adUnitId = adUnitId;
            }

            #region Load

            public void TryLoad(
                Action<string> onLoaded = null,
                Action<string> onFailed = null)
            {
                if (IsActive)
                {
                    Debug.Log($"Ad {_adUnitId} is already active.");
                    return;
                }
                
                _loadTimeOutId = TimerManager.Add(new TimerData(TimeOutDelay, onEndAction: () =>
                {
                    Debug.LogWarning($"Load Timeout: {_adUnitId} did not respond.");
                    FailLoad(_adUnitId);
                }));
                
                ClearLoadOneShot();
                
                _onLoadSuccessOneShot = onLoaded;
                _onLoadFailedOneShot = onFailed;

                Advertisement.Load(_adUnitId, this);
            }
            
            public void OnUnityAdsAdLoaded(string adUnitId)
            {
                TimerManager.Remove(_loadTimeOutId);
                //
                Debug.Log($"Ad {adUnitId} was loaded.");
                _onLoadSuccessOneShot?.Invoke(adUnitId);
                ClearLoadOneShot();
            }

            public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
            {
                Debug.Log($"Error loading {adUnitId}: {error} - {message}");

                FailLoad(adUnitId);
            }

            private void ClearLoadOneShot()
            {
                _onLoadSuccessOneShot = null;
                _onLoadFailedOneShot = null;
            }
            
            private void FailLoad(string adUnitId)
            {
                TimerManager.Remove(_loadTimeOutId);
                _onLoadFailedOneShot?.Invoke(adUnitId);
                ClearLoadOneShot();
            }

            #endregion
            
            #region Show

            public void TryShow(
                Action<string> onShowed = null,
                Action<string> onClicked = null,
                Action<string> onFailed = null,
                Action<string> onCompleted = null,
                Action<string> onSkipped = null)
            {
                ClearShowOneShot();
                
                _onShowSuccessOneShot = onShowed;
                _onShowClickedOneShot = onClicked;
                _onShowFailedOneShot = onFailed;
                _onShowCompletedOneShot = onCompleted;
                _onShowSkippedOneShot = onSkipped;

                _showTimeOutId = TimerManager.Add(new TimerData(TimeOutDelay, onEndAction: () =>
                {
                    FailShow(_adUnitId);
                    
                }));

                Advertisement.Show(_adUnitId, this);
            }
            
            public void OnUnityAdsShowStart(string adUnitId)
            {
                IsActive = true;
                TimerManager.Remove(_showTimeOutId);
                _onShowSuccessOneShot?.Invoke(adUnitId);
                Debug.Log($"Ad {_adUnitId} is now active.");
            }

            public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
            {
                Debug.Log($"Error showing {adUnitId}: {error} - {message}");

                FailShow(adUnitId);
            }

            public void OnUnityAdsShowClick(string adUnitId)
            {
                Debug.Log($"Ad {adUnitId} was clicked.");
                _onShowClickedOneShot?.Invoke(adUnitId);
            }

            public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState completion)
            {
                switch (completion)
                {
                    case UnityAdsShowCompletionState.COMPLETED:
                        _onShowCompletedOneShot?.Invoke(adUnitId);
                        Debug.Log($"Ad {adUnitId} was completed.");
                        break;

                    case UnityAdsShowCompletionState.SKIPPED:
                        _onShowSkippedOneShot?.Invoke(adUnitId);
                        Debug.Log($"Ad {adUnitId} was skipped.");
                        break;

                    case UnityAdsShowCompletionState.UNKNOWN:
                        Debug.Log($"Ad {adUnitId} has failed by an unkown reason.");
                        FailShow(adUnitId);
                        break;
                }

                IsActive = false;

                ClearShowOneShot();
            }
            
            private void FailShow(string adUnitId)
            {
                TimerManager.Remove(_showTimeOutId);
                _onShowFailedOneShot?.Invoke(adUnitId);
                IsActive = false;
                ClearShowOneShot();
            }

            private void ClearShowOneShot()
            {
                _onShowSuccessOneShot = null;
                _onShowFailedOneShot = null;
                _onShowCompletedOneShot = null;
                _onShowSkippedOneShot = null;
                _onShowClickedOneShot = null;
            }

            #endregion
        }
        
        public interface IBannerAd
        {
            public bool IsActive { get; }
            
            public void TryLoad(BannerPosition position = BannerPosition.CENTER, 
                Action onLoaded = null,
                Action<string> onFailed = null);

            public void TryShow(
                Action onShown = null,
                Action onClicked = null,
                Action onHidden = null
            );
            public void TryHide();
        }

        private class BannerAd : IBannerAd
        {
            private readonly string _adUnitId;
            public bool IsActive { get; private set; }
            
            public BannerAd(string addUnit)
            {
                _adUnitId = addUnit;
            }
            
            public void TryLoad(BannerPosition position = BannerPosition.CENTER, 
                Action onLoaded = null, 
                Action<string> onFailed = null)
            {
                if (IsActive)
                {
                    Debug.Log($"Ad {_adUnitId} is already active.");
                    return;
                }
                
                Advertisement.Banner.SetPosition(position);
                
                BannerLoadOptions options = new BannerLoadOptions
                {
                    loadCallback = ()=> onLoaded?.Invoke(),
                    errorCallback = (value)=> onFailed?.Invoke(value),
                };

                // Load the Ad Unit with banner content:
                Advertisement.Banner.Load(_adUnitId, options);
            }

            public void TryShow(
                Action onShown = null,
                Action onClicked = null,
                Action onHidden = null
                )
            {
                // Set up options to notify the SDK of show events:
                BannerOptions options = new BannerOptions
                {
                    clickCallback = ()=> onClicked?.Invoke(),
                    hideCallback = () =>
                    {
                        IsActive = false;
                        onHidden?.Invoke();
                    },
                    showCallback = () =>
                    {
                        IsActive = true;
                        onShown?.Invoke();
                    },
                };

                // Show the loaded Banner Ad Unit:
                Advertisement.Banner.Show(_adUnitId, options);
            }

            public void TryHide()
            {
                Advertisement.Banner.Hide();
            }
        }


        #endregion
        
        // ============================================== //

        private IUnityAd _interstitial;
        private IUnityAd _rewarded;
        private IBannerAd _banner;
        
#pragma warning disable CS0162 // Unreachable code detected
        
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        public static IUnityAd GetInterstitial()
        {
            return GameParameters.GameplayValues.AdsEnable ?  _instance._interstitial : null;
        }

        public static IUnityAd GetRewarded()
        {

            return GameParameters.GameplayValues.AdsEnable ?  _instance._rewarded : null;
        }

        public static IBannerAd GetBanner()
        {
            return GameParameters.GameplayValues.AdsEnable ?  _instance._banner : null;

        }
#else

        public static IUnityAd GetInterstitial()
        {
            return _instance._interstitial;   
        }

        public static IUnityAd GetRewarded()
        {
            return _instance._rewarded; 
        }

        public static IBannerAd GetBanner()
        {
            return _instance._banner; 
        }


#endif
#pragma warning restore CS0162 // Unreachable code detected

        private void Awake()
        {
            BootEvents.OnMainSystemRequestInitialize += Initialize;
        }

#pragma warning disable CS0162 // Unreachable code detected
        private void Initialize()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR

            if (GameParameters.GameplayValues.AdsEnable == false)
            {
                BootEvents.OnMainSystemRequestInitialize -= Initialize;
                BootEvents.MainSystemInitialized();
                return;
            }
#endif
            
            BootEvents.OnMainSystemRequestInitialize -= Initialize;
            //

#if UNITY_IOS

            _interstitial = new BaseAd("Interstitial_iOS");
            _rewarded = new BaseAd("Rewarded_iOS");
            _banner = new BannerAd("Banner_iOS");
            
#elif UNITY_ANDROID
            
            _interstitial = new BaseAd("Interstitial_Android");
            _rewarded = new BaseAd("Rewarded_Android");
            _banner = new BannerAd("Banner_Android");
            
#elif UNITY_EDITOR

            _interstitial = new BaseAd("Interstitial_Android");
            _rewarded = new BaseAd("Rewarded_Android");
            _banner = new BannerAd("Banner_Android");
            
#endif
            
            
            //
            BootEvents.MainSystemInitialized();
        }
#pragma warning restore CS0162 // Unreachable code detected
    }
}