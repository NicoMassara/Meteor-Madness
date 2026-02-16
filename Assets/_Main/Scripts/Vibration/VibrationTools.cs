using System;
using UnityEngine;

namespace MeteorMadness.Vibration
{
    public class VibrationTools
    {
        public static long GetDuration(DurationType duration)
        {
            return duration switch
            {
                DurationType.None => 0,
                DurationType.ExtraShort => 50,
                DurationType.MediumShort => 75,
                DurationType.Short => 100,
                DurationType.Medium => 250,
                DurationType.MediumLong => 500,
                DurationType.Long => 750,
                DurationType.ExtraLong => 850,
                DurationType.SuperLong => 1000,
                _ => GetDuration(DurationType.Medium)
            };
        }
        
        public static int GetIntensity(IntensityType intensity)
        {

            return intensity switch
            {
                IntensityType.None => 0,
                IntensityType.ExtraLight => 10,
                IntensityType.Light => 25,
                IntensityType.MediumLight => 40,
                IntensityType.Medium => 50,
                IntensityType.MediumHeavy => 75,
                IntensityType.Heavy => 128,
                IntensityType.ExtraHeavy => 200,
                IntensityType.FullHard => 255,
                _ => GetIntensity(IntensityType.Medium)
            };
        }
        
        public static VibrationData GetUIVibration(UIVibrationType type)
        {
            return type switch
            {
                UIVibrationType.UIButtonAccept => new VibrationData
                {
                    Duration = GetDuration(DurationType.Short),
                    Intensity = GetIntensity(IntensityType.Light)
                },
                UIVibrationType.UIButtonCancel => new VibrationData
                {
                    Duration = GetDuration(DurationType.Short),
                    Intensity = GetIntensity(IntensityType.MediumLight)
                },
                _ => GetUIVibration(UIVibrationType.UIButtonAccept)
            };
        }
        
        public static WaveformData CreateWaveformData(VibrationData[] data)
        {
            if (data == null || data.Length == 0)
            {
                return new WaveformData();
            }

            long[] durations = new long[data.Length + 1];
            int[] intensities = new int[data.Length + 1];

            // Initial delay
            durations[0] = 0;
            intensities[0] = 0;

            for (int i = 1; i <= data.Length; i++)
            {
                durations[i] = Math.Max(0, data[i - 1].Duration);
                intensities[i] = Math.Max(0, data[i - 1].Intensity);
            }

            return new WaveformData
            {
                Durations = durations,
                Intensities = intensities,
            };
        }
        
        public static long GetTotalDuration(long[] timings)
        {
            long total = 0;
            for (int i = 0; i < timings.Length; i++)
                total += timings[i];
            return total;
        }

    }
    
    public struct WaveformData
    {
        public long[] Durations;
        public int[] Intensities;
    }
    
    [Serializable]
    public class VibrationData
    {
        [Tooltip("In ms")]
        [Range(10,3000)]
        public long Duration;
        [Range(0,255)]
        public int Intensity;
    }
    
}