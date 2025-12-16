using _Main.Scripts.Interfaces.Sounds;
using Plugins.NicolasMassara.CustomSoundManager;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    public abstract class SoundBehaviour<T> : MonoBehaviour
    where T : ISoundComponent
    {
        protected T ComponentToSound => _componentToSound != null ? _componentToSound : SetComponentToSound();

        private T _componentToSound;

        private void Awake()
        {
            _componentToSound = GetComponent<T>();

            if (_componentToSound == null)
            {
                Debug.LogError($"{typeof(T)} does not have a sound component");
                this.enabled = false;
            }
        }

        protected SoundManager.GeneratedId PlaySound(ISoundSourceData soundClass, SoundManager.GeneratedId soundId = null)
        {
            if (soundClass == null)
                return null;

            if (soundId != null && soundId.IsActive)
            {
                ResumeSound(soundId);
                return soundId;
            }

            return SoundManager.PlaySound(soundClass, transform);
        }
        protected void StopSound(SoundManager.GeneratedId soundId)
        {
            if(IsIdValid(soundId) == false) return;
            
            SoundManager.StopSound(soundId);
        }
        protected void ResumeSound(SoundManager.GeneratedId soundId)
        {
            if(IsIdValid(soundId) == false) return;
            
            SoundManager.ResumeSound(soundId);
        }
        protected void PauseSound(SoundManager.GeneratedId soundId)
        {
            if(IsIdValid(soundId) == false) return;
            
            SoundManager.PauseSound(soundId);
        }

        // If ID is valid it means that is already in use and playing
        protected bool IsIdValid(SoundManager.GeneratedId soundId)
        {
            if (soundId == null)
            {
                //Debug.LogWarning($"Sound Id is null");
                return false;
            }

            if (soundId.IsActive == false)
            {
                //Debug.Log("Sound Id is not valid");
                return false;
            }

            return true;
        }

        protected void DeathFromParent(SoundManager.GeneratedId soundId)
        {
            if(IsIdValid(soundId) == false) return;
            
            SoundManager.DetachSoundFromParent(soundId);
        }

        private T SetComponentToSound()
        {
            _componentToSound = GetComponent<T>();

            return _componentToSound;
        }
        
    }
}