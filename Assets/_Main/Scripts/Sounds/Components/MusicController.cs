using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class MusicController
    {
        private ulong _playingId;
        private ulong _pausedId;
        
        // Add the possibility to pause and resume music
        // Mostly need for the GameMode whe is Paused
        // Add the Possibility to reduce the music volume
        
        // Use IDs
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
#pragma warning disable CS0067 // Event is never used
        public event Action<ulong> OnMusicPlaying;
        public event Action<ulong> OnMusicPaused;
        public event Action<ulong> OnMusicResumed;
        public event Action OnMusicStopped;
#pragma warning restore CS0067 // Event is never used
#endif
        
        public bool IsThisPlaying(SoundId soundId)
        {
            if (soundId == null) return false;
            if (soundId.Id == 0) return false;
            
            return _playingId == soundId.Id;
        }

        public bool IsThisPaused(SoundId soundId)
        {
            if (soundId == null) return false;
            if (soundId.Id == 0) return false;
            
            return _pausedId == soundId.Id;
        }

        public bool HasMusicPlaying()
        {
            return _playingId > 0;
        }

        public bool HasMusicPaused()
        {
            return _pausedId > 0;
        }

        public ulong Play(ulong toPlayId)
        {
            _playingId = toPlayId;
            OnMusicPlaying?.Invoke(_playingId);
            return _playingId;
        }
        public ulong Stop()
        {
            var temp = _playingId;
            _playingId = 0;
            OnMusicStopped?.Invoke();
            return temp;
        }

        public ulong Pause()
        {
            _pausedId = _playingId;
            _playingId = 0;
            OnMusicPaused?.Invoke(_pausedId);
            return _pausedId;
        }

        public ulong Resume()
        {
            _playingId = _pausedId;
            _pausedId = 0;
            OnMusicResumed?.Invoke(_playingId);
            return _playingId;
        }

        public void StopPaused()
        {
            _pausedId = 0;
            OnMusicPaused?.Invoke(_pausedId);
        }
    }
}