using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class DefeatAnimationSounds : MusicBehavior<IDefeatAnimationSounds>
    {
        [SerializeField] private SoundSourceDataSo music;
        private SoundManager.GeneratedId _musicId;
        
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