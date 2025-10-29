using System.Collections.Generic;

namespace _Main.Scripts.Sounds
{
    public class MusicController
    {
        private Dictionary<MusicType, SoundBehavior> _musicDictionary = new Dictionary<MusicType, SoundBehavior>();
        private MusicType _currentMusic;
        
        public void AddMusic(MusicType type, SoundBehavior soundBehavior)
        {
            _musicDictionary.TryAdd(type, soundBehavior);
        }

        public void PlayMusic(MusicType type)
        {
            if (HasMusicType(type))
            {
                if (type != _currentMusic)
                {
                    GetMusicBehavior(_currentMusic).StopSound();
                }

                _currentMusic = type;
                GetMusicBehavior(type).PlayAudio();
            }
        }

        public void StopCurrentMusic()
        {
            GetMusicBehavior(_currentMusic).StopSound();
        }
        
        public void PauseCurrentMusic()
        {
            GetMusicBehavior(_currentMusic).PauseSound();
        }

        public void SetMusicVolume(float volume = 1)
        {
            GetMusicBehavior(_currentMusic).SetVolumeMultiplier(volume);
        }

        private SoundBehavior GetMusicBehavior(MusicType type)
        {
            return _musicDictionary[type];
        }

        private bool HasMusicType(MusicType type)
        {
            return _musicDictionary.ContainsKey(type);
        }
    }
}