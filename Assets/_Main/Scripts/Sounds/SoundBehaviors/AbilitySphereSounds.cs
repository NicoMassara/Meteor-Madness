using MeteorMadness.GlobalValues.Interfaces.Sounds;
using MeteorMadness.Sounds.BaseBehaviors;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace MeteorMadness.Sounds.SoundBehaviors
{
    public class AbilitySphereSounds : SoundBehaviour<IAbilitySphereSounds>
    {
        [SerializeField] private SoundSourceDataSo movement;

        private SoundManager.GeneratedId _movementSoundId;

        private void Start()
        {
            ComponentToSound.OnStartSound += () =>
            {
                _movementSoundId = PlaySound(movement);
            };

            ComponentToSound.OnStopSound += () =>
            {
                StopSound(_movementSoundId);
            };
        }
    }
}