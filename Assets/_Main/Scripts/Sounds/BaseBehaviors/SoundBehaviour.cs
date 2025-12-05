using System;
using _Main.Scripts.CustomId;
using _Main.Scripts.Interfaces.Sounds;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public abstract class SoundBehaviour<T> : MonoBehaviour
    where T : ISoundComponent
    {
        protected T ComponentToSound => _componentToSound != null ? _componentToSound : SetComponentToSound();

        private T _componentToSound;

        protected SoundManager SoundManager => SoundManager.Instance;

        private void Awake()
        {
            _componentToSound = GetComponent<T>();

            if (_componentToSound == null)
            {
                Debug.LogError($"{typeof(T)} does not have a sound component");
                this.enabled = false;
            }
        }

        protected GeneratedId PlaySound(SoundClassSo soundClass)
        {
            if (soundClass == null)
                return null;
            
            return SoundManager.PlaySound(soundClass, transform);
        }
        protected void StopSound(GeneratedId soundId)
        {
            if(IsIdValid(soundId) == false) return;
            
            SoundManager.StopSound(soundId);
        }
        protected void ResumeSound(GeneratedId soundId)
        {
            if(IsIdValid(soundId) == false) return;
            
            SoundManager.ResumeSound(soundId);
        }
        protected void PauseSound(GeneratedId soundId)
        {
            if(IsIdValid(soundId)) return;
            
            SoundManager.PauseSound(soundId);
        }

        // If ID is valid it means that is already in use and playing
        protected bool IsIdValid(GeneratedId soundId)
        {
            if (soundId == null)
            {
                //Debug.LogWarning($"Sound Id is null");
                return false;
            }

            if (soundId.IsValid == false)
            {
                //Debug.Log("Sound Id is not valid");
                return false;
            }

            return true;
        }

        private T SetComponentToSound()
        {
            _componentToSound = GetComponent<T>();

            return _componentToSound;
        }
        
        

    }
}