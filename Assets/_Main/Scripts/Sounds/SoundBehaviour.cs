using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public class SoundBehaviour<T> : MonoBehaviour, ILoopableSound 
        where T : MonoBehaviour
    {
        protected T ComponentToSound { get; private set; }
        private SoundManager _soundManager;
        public event Action OnLoopFinished;

        private void Awake()
        {
            _soundManager = SoundManager.Instance;
            ComponentToSound = GetComponent<T>();
        }

        protected ulong PlaySound(SoundClassSo soundClass)
        {
            return _soundManager.PlaySound(soundClass, transform);
        }

        public void PlayMusic(ISoundData soundData)
        {
            _soundManager.PlayMusic(soundData);
        }

        public void StopSound(ulong soundId)
        {
            _soundManager.StopSound(soundId);
        }
        
        public void StopMusic()
        {
            _soundManager.StopMusic();
        }
    }
}