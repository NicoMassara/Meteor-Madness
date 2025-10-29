
using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundBehavior : ManagedBehavior, IPoolable<SoundBehavior>, ITrackedAudio
    {
        private bool _isPlaying;
        private bool _hasSoundClass;
        private bool _isUniqueClip;
        private bool _hasRandomPitch;
        private AudioSource _audioSource;
        public float VolumeMultiplier { get; private set; }
        public ISoundData SoundClass { get; private set; }
        
        public event Action<SoundBehavior> OnFinished;
        public event Action<SoundBehavior> OnRecycle;
        public void Recycle()
        {
            OnRecycle?.Invoke(this);
        }

        public void TriggerFinish()
        {
            OnFinished?.Invoke(this);
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            VolumeMultiplier = 1;
        }

        public void SetData(ISoundData soundClass)
        {
            if (soundClass != null)
            {
                _hasSoundClass = true;
                SoundClass = soundClass;
#if UNITY_EDITOR_WIN
                gameObject.name = SoundClass.ClassName;
#endif
                SetAudioData(SoundClass.SourceData);
            }
            else
            {
                _hasSoundClass = false;
                Debug.Log("No valid sound class");
            }
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        private void SetAudioData(AudioSourceData sourceData)
        {
            _isUniqueClip = SoundClass.IsUniqueClip;
            _hasRandomPitch = SoundClass.HasRandomPitch;
            _audioSource.clip = SoundClass.GetAudioClip();
            _audioSource.outputAudioMixerGroup = sourceData.mixerGroup;
            _audioSource.loop = sourceData.loop;
            _audioSource.bypassEffects = sourceData.ignoreEffects;
            _audioSource.bypassListenerEffects = sourceData.ignoreListenerEffects;
            _audioSource.bypassReverbZones = sourceData.ignoreReverbZones;
            _audioSource.volume = sourceData.volume;
            _audioSource.pitch = sourceData.pitch;
            _audioSource.priority = sourceData.priority;
            _audioSource.panStereo = sourceData.stereoPan;
            _audioSource.spatialBlend = sourceData.spatialBlend;
        }
        
        public void SetVolumeMultiplier(float multiplier)
        {
            VolumeMultiplier = multiplier;
            _audioSource.volume *= VolumeMultiplier;
        }

        public void PlaySound(float volumeMultiplier = 1)
        {
            if (!_hasSoundClass)
            {
                Debug.Log("Sound class is null");
                return;
            }

            SetVolumeMultiplier(volumeMultiplier);
             
            if (!_isUniqueClip)
            {
                _audioSource.clip = SoundClass.GetAudioClip();
            }
            
            if (_hasRandomPitch)
            {
                _audioSource.pitch = SoundClass.GetRandomPitch();
            }
            
            _audioSource?.Play();
            _isPlaying = true;
        }

        public void StopSound()
        {
            if (!_hasSoundClass)
            {
                //Debug.Log("Sound class is null");
                return;
            }
            
            _audioSource.Stop();
            _isPlaying = false;
        }

        public void PauseSound()
        {
            if (!_hasSoundClass)
            {
                //Debug.Log("Sound class is null");
                return;
            }
            
            _audioSource.Pause();
        }

        public bool GetIsPlaying()
        {
            return _hasSoundClass && _audioSource.isPlaying && _isPlaying;
        }


    }
}