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
            GetComponentToSound.OnTutorialEnable += () =>
            {
                _musicId = PlaySound(music);
            };
            
            GetComponentToSound.OnTutorialFinished += () =>
            {
                StopSound(_musicId);
            };
        }
    }
}