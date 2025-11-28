using _Main.Scripts.CustomId;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Sphere.Components
{
    public class AbilitySphereSounds : SoundBehaviour<AbilitySphereView>
    {
        [SerializeField] private SoundClassSo movement;

        private GeneratedId _movementSoundId;

        private void Start()
        {
            ComponentToSound.OnValuesChanged += (value) =>
            {
                _movementSoundId = PlaySound(movement);
            };

            ComponentToSound.OnRecycle += (value) =>
            {
                StopSound(_movementSoundId);
            };
        }
    }
}