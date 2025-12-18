namespace _Main.Scripts.AdsSystem
{
    public class AdsTools
    {
        public static bool GetAreAdsDisable()
        {
#pragma warning restore CS0162 // Unreachable code detected
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            return !GameParameters.GameplayValues.AdsEnable;
#else
            return false;
#endif
#pragma warning restore CS0162 // Unreachable code detected

        }
    }
}