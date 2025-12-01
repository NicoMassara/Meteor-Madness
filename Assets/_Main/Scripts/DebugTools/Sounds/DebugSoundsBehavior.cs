using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.DebugTools.Sounds
{
    public class DebugSoundsBehavior : SoundBehaviour<IDebugSounds>
    {
        [Header("Sound Data")]
        [SerializeField] private SoundClassSo soundData;

        private GeneratedId _soundId;
        
        private void Start()
        {
            ComponentToSound.OnSoundPlayed += () =>
            {
                _soundId = PlaySound(soundData);
            };
            
            ComponentToSound.OnSoundStopped += () =>
            {
                StopSound(_soundId);
            };
        }
    }
}