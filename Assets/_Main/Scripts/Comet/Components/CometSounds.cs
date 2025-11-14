using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Comet.Components
{
    public class CometSounds : SoundBehaviour<CometView>
    {
        [SerializeField] private SoundClassSo movement;

        private SoundId _movementSoundId;

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