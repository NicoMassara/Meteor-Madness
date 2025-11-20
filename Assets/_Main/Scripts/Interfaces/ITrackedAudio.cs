using System;

namespace _Main.Scripts.Interfaces
{
    public interface ITrackedAudio
    {
        public string AudioName { get; }
        public bool IsLooping { get; }

        public void PlayAudio();
        public bool GetIsPlaying();
        public bool GetIsPaused();
        public void TriggerFinish();
    }
}