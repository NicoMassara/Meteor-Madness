using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class ProjectileSounds : SoundBehaviour<IProjectileSounds>
    {
        [SerializeField] private SoundClassSo movement;

        private GeneratedId _movementSoundId;

        private void Start()
        {
            ComponentToSound.OnStart += () =>
            {
                _movementSoundId = PlaySound(movement);
            };

            ComponentToSound.OnStop += () =>
            {
                StopSound(_movementSoundId);
            };
        }
    }
}