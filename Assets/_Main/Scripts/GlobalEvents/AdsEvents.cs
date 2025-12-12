using System;

namespace _Main.Scripts.GlobalEvents
{
    public class AdsEvents
    {
        // Initialize Ads
        
#pragma warning disable CS0067 // Event is never used
        public static event Action OnInitializeAds;
#pragma warning restore CS0067 // Event is never used
        
        public static void InitializeAds()
        {
            OnInitializeAds?.Invoke();
        }
        
        // Notify Ads Initialized
        public static event Action OnAdsInitialized;

        public static void TriggerAdsInitialized()
        {
            OnAdsInitialized?.Invoke();
        }
    }
}