using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.MyTest.Sounds
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
                _soundId = PlaySound(soundData,_soundId);
            };
            
            ComponentToSound.OnSoundPaused += () =>
            {
                PauseSound(_soundId);
            };

            ComponentToSound.OnSoundResumed += () =>
            {
                ResumeSound(_soundId);
            };
            
            ComponentToSound.OnSoundStopped += () =>
            {
                StopSound(_soundId);
            };
        }
    }
}