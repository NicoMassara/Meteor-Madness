namespace _Main.Scripts.AdsSystem
{
    public class AdsTools
    {
        public static bool GetAreAdsDisable()
        {
#pragma warning restore CS0162 // Unreachable code detected
            return GameParameters.GameplayValues.AdsEnable == false;
#pragma warning restore CS0162 // Unreachable code detected

        }
    }
}