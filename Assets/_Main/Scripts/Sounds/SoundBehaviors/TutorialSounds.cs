using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class TutorialSounds : SoundBehaviour<ITutorialSounds>
    {
        [SerializeField] private SoundClassSo music;

        private GeneratedId _musicId;

        private void Start()
        {
            ComponentToSound.OnTutorialEnable += () =>
            {
                _musicId = PlaySound(music);
            };
            
            ComponentToSound.OnTutorialFinished += () =>
            {
                StopSound(_musicId);
            };
        }
    }
}