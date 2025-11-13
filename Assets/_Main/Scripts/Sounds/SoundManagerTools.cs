using System;

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
    }
}