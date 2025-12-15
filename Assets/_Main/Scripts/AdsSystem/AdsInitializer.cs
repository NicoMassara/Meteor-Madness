using _Main.Scripts.GlobalEvents;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.Advertisements;

namespace _Main.Scripts.AdsSystem
{
    public class AdsInitializer : MonoBehaviour
    {
#if UNITY_ANDROID || UNITY_IOS
        
        private const bool TestMode = true;

        private void Awake()
        {
            AdsEvents.OnInitializeAds += InitializeAds;
        }

#pragma warning disable CS0162 // Unreachable code detected
        private void InitializeAds()
        {
            AdsEvents.OnInitializeAds -= InitializeAds;
            //
#if DEVELOPMENT_BUILD || UNITY_EDITOR

            if (GameParameters.GameplayValues.AdsEnable == false)
            {
                AdsEvents.TriggerAdsInitialized();
                AdsEvents.OnInitializeAds -= InitializeAds;
                return;
            }
#endif
            
            LevelPlay.ValidateIntegration();
            
            LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
            LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
            
            LevelPlay.Init(AdConfig.AppKey);
        }

        private void SdkInitializationFailedEvent(LevelPlayInitError input)
        {
            Debug.LogError($"Ad Initialization failed: {input}");
        }

        private void SdkInitializationCompletedEvent(LevelPlayConfiguration input)
        {
            Debug.Log("Ad Initialization Completed");
            AdsEvents.TriggerAdsInitialized();
        }
#pragma warning restore CS0162 // Unreachable code detected

#endif
    }
    
}