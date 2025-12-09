using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class TutorialSounds : MusicBehavior<ITutorialSounds>
    {
        [SerializeField] private SoundClassSo music;

        private GeneratedId _musicId;

        private void Start()
        {
            ComponentToSound.OnTutorialEnable += () =>
            {
                StopAllMusic();
                _musicId = PlaySound(music);
            };
            
            ComponentToSound.OnTutorialFinished += () =>
            {
                StopAllMusic();
            };
        }
    }
}