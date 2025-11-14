using System;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class MusicController
    {
        private readonly VolumeChanger _volumeChanger = new VolumeChanger();
        private SoundComponent _currentMusic;
        private SoundComponent _nextMusic;
        
        public event Action<string> OnMusicLeaving;
        public event Action<string> OnMusicPlaying;
        public event Action<string> OnMusicArriving;
        
        private class VolumeChanger
        {
            private const float LerpTime = 1f;
            private SoundComponent _sound;
            private float _targetVolume;
            private float _startVolume;
            
            private float _currentVolume;
            private float _elapsedTime;
            private bool _hasVolumeToChange;
            public event Action<SoundComponent> _onEnd;
            
            public void IncreaseVolume(SoundComponent sound,Action<SoundComponent> onEnd)
            {
                sound.PlayAudio();
                SetVolumeToChange(sound, 0, 1, onEnd);
            }

            public void DecreaseVolume(SoundComponent sound, Action<SoundComponent> onEnd)
            {
                SetVolumeToChange(sound, 1, 0, onEnd);
            }


            private void SetVolumeToChange(SoundComponent sound, float startVolume, float targetVolume, Action<SoundComponent> onEnd)
            {
                _sound = sound;
                _targetVolume = targetVolume;
                _startVolume = startVolume;
                _onEnd = onEnd;
                _sound.SetVolumeMultiplier(_startVolume);
                
                _hasVolumeToChange = true;
            }
            
            public void ChangeVolume(float deltaTime)
            {
                if(_hasVolumeToChange == false) return;
                
                _elapsedTime += deltaTime;
                var timeRatio = _elapsedTime / LerpTime;
                
                _currentVolume = Mathf.Lerp(_startVolume, _targetVolume, timeRatio);
                _sound.SetVolumeMultiplier(_currentVolume);

                if (timeRatio >= 1)
                {
                    _hasVolumeToChange = false;
                    _elapsedTime = 0;
                    _onEnd?.Invoke(_sound);
                }
            }
        }
        
        public void Execute(float deltaTim)
        {
            _volumeChanger?.ChangeVolume(deltaTim);
        }
        
        public void StopMusic()
        {
            OnMusicLeaving?.Invoke(_currentMusic.SoundClass.ClassName);
            _volumeChanger.DecreaseVolume(_currentMusic, OnMusicStop);
        }

        public void PlayMusic(SoundComponent music)
        {
            if(music == null) return;

            if (_currentMusic == null)
            {
                OnMusicArriving?.Invoke(music.SoundClass.ClassName);
                _volumeChanger.IncreaseVolume(music, MusicAdded);
            }
            else if (_currentMusic != null && _nextMusic == null)
            {
                _nextMusic = music;
                OnMusicLeaving?.Invoke(_currentMusic.SoundClass.ClassName);
                _volumeChanger.DecreaseVolume(_currentMusic, MusicLeaving);
            }
            else
            {
                Debug.LogWarning("Already has a New Music in Queue");
            }
        }

        private void MusicAdded(SoundComponent music)
        {
            SetCurrentMusic(music);
            OnMusicArriving?.Invoke("None");
        }

        private void MusicLeaving(SoundComponent music)
        {
            music.StopSound();
            _volumeChanger.IncreaseVolume(_nextMusic, MusicAdded);
            OnMusicLeaving?.Invoke("None");
        }

        private void OnMusicStop(SoundComponent music)
        {
            OnMusicLeaving?.Invoke("None");
            SetCurrentMusic(null);
            music.StopSound();
        }

        private void SetCurrentMusic(SoundComponent music)
        {
            _currentMusic = music;
            
            var musicName = _currentMusic != null ? _currentMusic.SoundClass.ClassName : "None";
            
            OnMusicPlaying?.Invoke(musicName);
        }
    }
}