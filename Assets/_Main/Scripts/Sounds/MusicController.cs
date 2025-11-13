using System.Collections.Generic;

namespace _Main.Scripts.Sounds
{
    public class MusicController
    {
        private SoundComponent _currentMusic;
        
        public void Execute()
        {
            
        }

        public void PlayMusic(SoundComponent music)
        {
            if(music == null) return;
            
            if (_currentMusic == null)
            {
                _currentMusic = music;
            }
            else
            {
                _currentMusic.StopSound();
                _currentMusic = music;
            }
            
            _currentMusic.PlayAudio();
        }

        public void StopMusic()
        {
            _currentMusic?.StopSound();
        }

        private void SetCurrentMusic(SoundComponent music)
        {

        }
    }
}