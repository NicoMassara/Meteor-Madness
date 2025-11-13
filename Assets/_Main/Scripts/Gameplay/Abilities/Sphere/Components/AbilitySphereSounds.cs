using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Sphere.Components
{
    public class AbilitySphereSounds : SoundBehaviour<AbilitySphereView>
    {
        [SerializeField] private SoundClassSo movement;

        private ulong _movementSoundId;

        private void Start()
        {
            GetComponentToSound.OnValuesChanged += (value) =>
            {
                _movementSoundId = PlaySound(movement);
            };

            GetComponentToSound.OnRecycle += (value) =>
            {
                StopSound(_movementSoundId);
            };
        }
    }
}