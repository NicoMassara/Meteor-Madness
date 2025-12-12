using _Main.Scripts.GlobalEvents;
using UnityEngine;
using UnityEngine.Advertisements;

namespace _Main.Scripts.AdsSystem
{
    public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
    {
        private const bool TestMode = true;
        private string _gameId;

        private void Awake()
        {
            AdsEvents.OnInitializeAds += InitializeAds;
        }

        private void InitializeAds()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR

            if (GameParameters.GameplayValues.AdsEnable == false)
            {
                AdsEvents.TriggerAdsInitialized();
                AdsEvents.OnInitializeAds -= InitializeAds;
                return;
            }
#endif
            
            
#if UNITY_IOS
            _gameId = 6001500;
#elif UNITY_ANDROID
            _gameId = "6001501";
#elif UNITY_EDITOR
            _gameId = "6001501"; //Only for testing the functionality in the Editor
#endif
            
            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, TestMode, this);
            }
            else if(!Advertisement.isSupported)
            {
                Debug.LogError($"Advertisement is not supported: {_gameId}");
            }
            
            AdsEvents.OnInitializeAds -= InitializeAds;
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
            AdsEvents.TriggerAdsInitialized();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }
    }
}