using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
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