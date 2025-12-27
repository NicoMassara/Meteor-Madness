using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class TutorialSounds : MusicBehavior<ITutorialSounds>
    {
        [SerializeField] private SoundSourceDataSo music;

        private SoundManager.GeneratedId _musicId;

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