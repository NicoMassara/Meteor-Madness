using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds.Components
{
    public class ProjectileSounds : SoundBehaviour<IProjectileSounds>
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