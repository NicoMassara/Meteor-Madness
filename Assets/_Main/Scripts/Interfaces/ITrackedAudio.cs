using System;

namespace _Main.Scripts.Interfaces
{
    public interface ITrackedAudio
    {
        public bool GetIsPlaying();
        public void TriggerFinish();
    }
}