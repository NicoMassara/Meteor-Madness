using System;

namespace _Main.Scripts.Interfaces
{
    public interface ITrackedAudio
    {
        public string AudioName { get; }
        public ISoundData SoundClass { get;}
        public bool IsLooping { get; }

        public void PlayAudio();
        public void SetVolumeMultiplier(float multiplier);
        public void StopSound();
        public void PauseSound();
        public void ResumeSound();
        public bool GetIsPlaying();
        public bool GetIsPaused();
        public void TriggerFinish();
    }
}