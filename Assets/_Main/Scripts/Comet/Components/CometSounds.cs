using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Comet.Components
{
    public class CometSounds : SoundBehaviour<CometView>
    {
        [SerializeField] private SoundClassSo movement;

        private ulong _movementSoundId;

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