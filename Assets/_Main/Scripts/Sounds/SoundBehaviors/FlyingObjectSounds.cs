using MeteorMadness.Contracts.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class FlyingObjectSounds : SoundBehaviour<IFlyingObjectSounds>
    {
        [SerializeField] private SoundSourceDataSo movement;

        private SoundManager.GeneratedId _movementSoundId;

        private void Start()
        {
            ComponentToSound.OnStart += () =>
            {
                _movementSoundId = PlaySound(movement);
            };

            ComponentToSound.OnStop += () =>
            {
                DeathFromParent(_movementSoundId);
                StopSound(_movementSoundId);
            };
        }
    }
}