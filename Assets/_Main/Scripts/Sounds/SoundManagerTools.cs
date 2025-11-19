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
                _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
            };
        }

        public static float GetDbFrom01Value(float value)
        {
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        }
    }
}