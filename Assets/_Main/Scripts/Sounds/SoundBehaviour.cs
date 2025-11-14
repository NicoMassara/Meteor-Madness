using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public abstract class SoundBehaviour<T> : MonoBehaviour 
        where T : MonoBehaviour
    {
        protected T GetComponentToSound => _componentToSound != null ? _componentToSound : SetComponentToSound();

        private T _componentToSound;

        private SoundManager _soundManager;

        private void Awake()
        {
            _soundManager = SoundManager.Instance;
            _componentToSound = GetComponent<T>();
        }

        protected SoundId PlaySound(SoundClassSo soundClass)
        {
            return _soundManager.PlaySound(soundClass, transform);
        }

        protected void PlayMusic(ISoundData soundData)
        {
            _soundManager.PlayMusic(soundData);
        }

        protected void StopSound(SoundId soundId)
        {
            _soundManager.StopSound(soundId);
        }
        
        protected void PlayUISound(UISoundType soundType)
        {
            _soundManager.PlayUISound(soundType);
        }
        
        public void StopMusic()
        {
            _soundManager.StopMusic();
        }

        private T SetComponentToSound()
        {
            _componentToSound = GetComponent<T>();

            return _componentToSound;
        }

    }
}