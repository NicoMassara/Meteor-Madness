using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class DefeatAnimationSounds : MusicBehavior<IDefeatAnimationSounds>
    {
        [SerializeField] private SoundClassSo music;
        private GeneratedId _musicId;
        
        private void Start()
        {
            ComponentToSound.OnPlayMusic += () =>
            {
                _musicId = PlayMusic(music, _musicId, true);
            };

            ComponentToSound.OnStopMusic += () =>
            {
                StopAllMusic();
            };
        }
    }
}