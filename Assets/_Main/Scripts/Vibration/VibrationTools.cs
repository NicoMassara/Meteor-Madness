using System;

namespace _Main.Scripts.Vibration
{
    public class VibrationTools
    {
        public static long GetDuration(VibrationDurationType duration)
        {
            return duration switch
            {
                VibrationDurationType.ExtraShort => 50,
                VibrationDurationType.MediumShort => 75,
                VibrationDurationType.Short => 100,
                VibrationDurationType.Medium => 250,
                VibrationDurationType.MediumLong => 500,
                VibrationDurationType.Long => 750,
                VibrationDurationType.ExtraLong => 850,
                VibrationDurationType.SuperLong => 1000,
                _ => GetDuration(VibrationDurationType.Medium)
            };
        }
        
        public static int GetIntensity(VibrationIntensityType intensity)
        {

            return intensity switch
            {
                VibrationIntensityType.ExtraLight => 10,
                VibrationIntensityType.Light => 25,
                VibrationIntensityType.MediumLight => 40,
                VibrationIntensityType.Medium => 50,
                VibrationIntensityType.MediumHeavy => 75,
                VibrationIntensityType.Heavy => 128,
                VibrationIntensityType.ExtraHeavy => 200,
                VibrationIntensityType.FullHard => 255,
                _ => GetIntensity(VibrationIntensityType.Medium)
            };
        }
        
        public static VibrationData GetType(VibrationType type)
        {
            return type switch
            {
                VibrationType.UIButtonAccept => new VibrationData
                {
                    Duration = GetDuration(VibrationDurationType.Short),
                    Intensity = GetIntensity(VibrationIntensityType.Light)
                },
                VibrationType.UIButtonCancel => new VibrationData
                {
                    Duration = GetDuration(VibrationDurationType.Short),
                    Intensity = GetIntensity(VibrationIntensityType.MediumLight)
                },
                _ => GetType(VibrationType.UIButtonAccept)
            };
        }
    }
}