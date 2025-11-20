using System;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class SoundManagerTools
    {
        public static int GetChannelLimit(SoundChannel channel)
        {
            return channel switch
            {
                SoundChannel.Sfx => 5,
                SoundChannel.Collision => 1,
                SoundChannel.Deflection => 1,
                SoundChannel.UI => 3,
                SoundChannel.Music => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
            };
        }

        public static float GetDbFrom01Value(float value)
        {
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        }

        public static float GetFadeFromType(FadeType type, float startValue, float targetValue, 
            float expStrength, float ratio)
        {
            return type switch
            {
                FadeType.Exp => Mathf.Lerp(startValue, targetValue, 1f - Mathf.Exp(-expStrength * ratio)),
                FadeType.Log => 1f - Mathf.Log10(1f + expStrength * ratio) / Mathf.Log10(1f + expStrength),
                _ => GetFadeFromType(FadeType.Exp, startValue, targetValue, expStrength, ratio)
            };
        }
    }
}