using System;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundBehavior : MonoBehaviour
    {
        [SerializeField] private SoundClassSo soundDataSo;
        private bool _isPlaying;
        private bool _hasSoundClass;
        private bool _isUniqueClip;
        private bool _hasRandomPitch;
        private AudioSource _audioSource;
        public float VolumeMultiplier { get; private set; }
        public SoundClassSo SoundClass { get; private set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            VolumeMultiplier = 1;
            SetSoundClass(soundDataSo);
            
            Debug.Log($"{_audioSource.clip.name} has been initialized");
        }

        private void Start()
        {
        }

        private void SetSoundClass(SoundClassSo soundClass)
        {
            if (soundClass)
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
        
        private void SetVolumeMultiplier(float multiplier)
        {
            VolumeMultiplier = multiplier;
            
            if (_audioSource != null)
            {
                _audioSource.volume *= VolumeMultiplier;
            }
        }

        public void PlaySound(float volumeMultiplier = 1, float pitch = 1)
        {
            if (!_hasSoundClass)
            {
                Debug.Log("Sound class is null");
                return;
            }
            
            if (_audioSource == null)
            {
                Debug.Log("Audio Source is null");
                return;
            }

            SetVolumeMultiplier(volumeMultiplier);
            if (!_isUniqueClip)
            {
                _audioSource.clip = SoundClass.GetAudioClip();
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (pitch != 1)
            {
                _audioSource.pitch = pitch;
            }
            else if (_hasRandomPitch)
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