
using System;
using _Main.Scripts.Interfaces;
using NicolasMassara.CustomActionManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Sounds
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundSource : ManagedBehavior, IPoolable<SoundSource>, ITrackedAudio, IUpdatable
    {
        private class VolumeChanger
        {
            private class ChangeVolumeAction : IQueueAction
            {
                protected readonly SoundSource Sound;
                private readonly float _startVolume;
                private readonly float _targetVolume;
                private float _elapsedTime;
                private readonly FadeData _fadeData;

                public ActionStatus CurrentStatus { get; private set; } = ActionStatus.Idle;
                
                
                public ChangeVolumeAction(SoundSource sound, float targetVolume, float startVolume, FadeData fadeData)
                {
                    Sound = sound;
                    _targetVolume = targetVolume;
                    _startVolume = startVolume;
                    _fadeData = fadeData;
                }

                public virtual void OnStart()
                {
                    if (Sound == null)
                    {
                        CurrentStatus = ActionStatus.Failure;
                        _elapsedTime = Mathf.Infinity;
                        Debug.LogError("Sound component is null");
                        return;
                    }
                }

                public virtual ActionStatus OnUpdate(float deltaTime)
                {
                    if (_elapsedTime < _fadeData.FadeTime)
                    {
                        _elapsedTime += deltaTime;
                        var ratio = _elapsedTime / _fadeData.FadeTime;

                        var current = SoundManagerTools.GetFadeFromType(_fadeData.FadeType,
                            _startVolume, _targetVolume,
                            _fadeData.ExpStrength, ratio);
                        
                        Sound.SetVolumeMultiplier(current);
                        
                        if (ratio >= 1f)
                        {
                            Sound.SetVolumeMultiplier(1);
                            CurrentStatus = ActionStatus.Success;
                        }
                    }
                    
                    return CurrentStatus;
                }

                public void OnInterrupt()
                {

                }

                public IQueueAction Copy()
                {
                    return new ChangeVolumeAction(Sound, _targetVolume, _startVolume, _fadeData);
                }
            }

            public static void ChangeVolume(SoundSource sound, float targetVolume, float startVolume, 
                FadeData fadeData, Action onStart = null, Action onEnd = null)
            {
                var actions = ActionBuilder.Start()
                    .Do(new ChangeVolumeAction(sound, targetVolume, startVolume, fadeData))
                    .WrapLast(a => new CallbackWrapperAction(a, onStart,onEnd))
                    .Then(new WaitFramesAction(1))
                    .Build();

                ActionManager.Add(actions);
            }

        }
        
        private bool _isPlaying;
        private bool _isPaused;
        private bool _hasSoundClass;
        private bool _isUniqueClip;
        private bool _hasRandomPitch;
        private AudioSource _audioSource;
        public float VolumeMultiplier { get; private set; }
        public ISoundData SoundClass { get; private set; }
        public string AudioName => SoundClass.ClassName;
        public bool IsLooping => SoundClass.DoesLoop;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Always;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        public event Action<SoundSource> OnFinished;
        public event Action<SoundSource> OnRecycle;
        
        public void Recycle()
        {
            transform.parent = null;
            OnRecycle?.Invoke(this);
        }

        public void TriggerFinish()
        {
            Recycle();
            
            if (SoundClass.Is3DSound)
            {
                transform.parent = null;
            }

            OnFinished?.Invoke(this);
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            VolumeMultiplier = 1;
        }
        
        public void ExecuteUpdate(float deltaTime)
        {
            if (_isPlaying && !_isPaused)
            {
                if (GetFadeOutRatio() >= 0.999f)
                {
                    StopSound();
                }
            }
        }

        public void SetData(ISoundData soundClass)
        {
            if (soundClass != null)
            {
                _hasSoundClass = true;
                SoundClass = soundClass;
#if UNITY_EDITOR_WIN
                gameObject.name = $"Sound_{SoundClass.ClassName}";
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
            _audioSource.volume = VolumeMultiplier;
        }

        public void PlayAudio()
        {
            if (!_hasSoundClass)
            {
                Debug.Log("Sound class is null");
                return;
            }
            
            //Debug.Log($"Playing {SoundClass.ClassName}");
            
            if (!_isUniqueClip)
            {
                _audioSource.clip = SoundClass.GetAudioClip();
            }
            
            if (_hasRandomPitch)
            {
                _audioSource.pitch = SoundClass.GetRandomPitch();
            }
            
            _isPlaying = true;
            _isPaused = false;
            
            VolumeChanger.ChangeVolume(this,1,0,
                SoundClass.FadeOutData, 
                onStart: () =>
                {
                    _audioSource?.Play();
                });
        }

        public void StopSound()
        {
            if (!_hasSoundClass || !_isPlaying)
            {
                //Debug.Log("Sound class is null");
                return;
            }
            
            _isPlaying = false;
            VolumeChanger.ChangeVolume(this,0f,VolumeMultiplier,
                SoundClass.FadeOutData, null, 
                onEnd: () =>
                {
                    _audioSource?.Stop();
                });

        }

        public void PauseSound()
        {
            if (!_hasSoundClass || _isPaused)
            {
                //Debug.Log("Sound class is null");
                return;
            }
            
            _isPaused = true;
            VolumeChanger.ChangeVolume(this,0f,VolumeMultiplier,
                SoundClass.FadeOutData, null, 
                onEnd: () =>
                {
                    _audioSource?.Pause();
                });
        }

        public void ResumeSound()
        {
            if (!_hasSoundClass || !_isPaused)
            {
                //Debug.Log("Sound class is null");
                return;
            }
            
            _isPaused = false;
            VolumeChanger.ChangeVolume(this,1f,0,
                SoundClass.FadeOutData, 
                onStart: () =>
                {
                    _audioSource?.Play();
                });
        }

        public bool GetIsPlaying()
        {
            return _hasSoundClass && (_audioSource.isPlaying || _isPlaying);
        }
        
        public bool GetIsPaused()
        {
            return _hasSoundClass && _isPaused;
        }

        private float GetPlayRatio()
        {
            if (_audioSource == null || _audioSource.clip == null) return 0;
            
            return Mathf.Clamp01(_audioSource.time / _audioSource.clip.length);
        }

        public float GetClipLenght()
        {
            if (_audioSource == null || _audioSource.clip == null) return 0;
            return _audioSource.clip.length;
        }

        private float GetFadeOutRatio()
        {
            if (_audioSource == null ||
                _audioSource.clip == null ||
                SoundClass == null) return 0;
            
            return Mathf.Clamp01(_audioSource.time / (_audioSource.clip.length - SoundClass.FadeOutData.FadeTime));
        }
    }
}