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


        public static Tuple<long[], int[]> CreateWaveformData(VibrationData[] data)
        {
            if (data == null || data.Length == 0)
            {
                return new Tuple<long[], int[]>( new long[]{0}, new int[]{0});
            }
            
            int offset = 1;
            long[] duration = new long[data.Length + 1];

            duration[0] = 0;

            for (int i = 1; i < duration.Length; i++)
            {
                duration[i] = Math.Max(0, data[i].Duration);
            }
            
            int[] intensity = new int[data.Length + 1];

            intensity[0] = 0;

            for (int i = 1; i < duration.Length; i++)
            {
                intensity[i] = Math.Max(0, data[i].Intensity);
            }
            
            return new Tuple<long[], int[]>(duration, intensity);
        }
        
        public static long GetTotalDuration(long[] timings)
        {
            long total = 0;
            for (int i = 0; i < timings.Length; i++)
                total += timings[i];
            return total;
        }

    }
    
    [Serializable]
    public class VibrationData
    {
        [Tooltip("In ms")]
        [Range(10,3000)]
        public long Duration;
        [Range(1,255)]
        public int Intensity;
    }
    
}