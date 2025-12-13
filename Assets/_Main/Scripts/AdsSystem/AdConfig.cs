namespace _Main.Scripts.AdsSystem
{
    public static class AdConfig
    {
#if UNITY_ANDROID || UNITY_IOS
        
        public static string AppKey => GetAppKey();
        public static string BannerAdUnitId => GetBannerAdUnitId();
        public static string InterstitalAdUnitId => GetInterstitialAdUnitId();
        public static string RewardedVideoAdUnitId => GetRewardedVideoAdUnitId();

        private static string GetAppKey()
        {
#if UNITY_ANDROID
            return "24905fab5";
#elif UNITY_IPHONE
            return "unexpected_platform";
#else
            return "unexpected_platform";
#endif
        }

        private static string GetBannerAdUnitId()
        {
#if UNITY_ANDROID
            return "7t22k1u1fop0b2vh";
#elif UNITY_IPHONE
            return "iep3rxsyp9na3rw8";
#else
            return "unexpected_platform";
#endif
        }
        private static string GetInterstitialAdUnitId()
        {
#if UNITY_ANDROID
            return "qlry4io4xjikjq4r";
#elif UNITY_IPHONE
            return "wmgt0712uuux8ju4";
#else
            return "unexpected_platform";
#endif
        }

        private static string GetRewardedVideoAdUnitId()
        {
#if UNITY_ANDROID
            return "sdlet675qz8f2iwk";
#elif UNITY_IPHONE
            return "qwouvdrkuwivay5q";
#else
            return "unexpected_platform";
#endif
        }

#endif
    }
}