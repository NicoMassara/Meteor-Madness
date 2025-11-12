using System;

namespace _Main.Scripts.Vibration
{
    public class VibrationTools
    {
        public static long GetDuration(VibrationDurationType duration)
        {
            return duration switch
            {
                VibrationDurationType.Short => 100,
                VibrationDurationType.Medium => 250,
                VibrationDurationType.Long => 1000,
                _ => GetDuration(VibrationDurationType.Medium)
            };
        }
        
        public static int GetIntensity(VibrationIntensityType intensity)
        {

            return intensity switch
            {
                VibrationIntensityType.Light => 75,
                VibrationIntensityType.Medium => 128,
                VibrationIntensityType.Heavy => 200,
                _ => GetIntensity(VibrationIntensityType.Medium)
            };
        }
        
        public static VibrationData GetType(VibrationType type)
        {
            return type switch
            {
                VibrationType.Button => new VibrationData
                {
                    Duration = GetDuration(VibrationDurationType.Short),
                    Intensity = GetIntensity(VibrationIntensityType.Light)
                },
                _ => GetType(VibrationType.Button)
            };
        }
    }
}