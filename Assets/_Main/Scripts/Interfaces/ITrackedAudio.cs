using System;

namespace _Main.Scripts.Interfaces
{
    public interface ITrackedAudio
    {
        public string AudioName { get; }

        public void PlayAudio();
        public bool GetIsPlaying();
        public void TriggerFinish();
    }
}