using System;

namespace MeteorMadness.GlobalValues.Events
{
    public class AdsEvents
    {
        // Initialize Ads
        public static event Action OnInitializeAds;
        
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
        
        // Banner Events
        public static event Action Banner_OnLoad;
        public static void Banner_TriggerLoad() => Banner_OnLoad?.Invoke();
        public static event Action Banner_OnShow;
        public static void Banner_TriggerShow() => Banner_OnShow?.Invoke();
        public static event Action Banner_OnHide;
        public static void Banner_TriggerHide() => Banner_OnHide?.Invoke();
        public static event Action Banner_OnDestroy;
        public static void Banner_TriggerDestroy() => Banner_OnDestroy?.Invoke();
        
        // Rewarded Events
        public static event Action Rewarded_OnLoad;
        public static void Rewarded_TriggerLoad() => Rewarded_OnLoad?.Invoke();
        public static event Action Rewarded_OnShow;
        public static void Rewarded_TriggerShow() => Rewarded_OnShow?.Invoke();
        
        public static event Action Rewarded_Reward;
        public static void Rewarded_TriggerReward() => Rewarded_Reward?.Invoke();
        
        
        



    }
}