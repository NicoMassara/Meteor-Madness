using System;
using System.Collections;
using UnityEngine;

namespace Plugins.NicolasMassara.CustomSoundManager
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundSource : MonoBehaviour, IPoolableSound, ISoundSource
    {
        private SoundState _state = SoundState.Stopped;

        private AudioSource _audioSource;
        private ISoundSourceData _soundSourceData;
        private Coroutine _fadeRoutine;
        private const float MinAudible = 0.001f;

        private bool _doesFadeOut;
        private bool _doesFadeIn;
        private bool _doesMove;
        
        // Volume Values
        private float _fadeVolume;
        private float _userVolume;
        private float _contextVolume;

        #region ISoundSource Values
        public bool DoesLoop { get; private set; }
        public SoundChannel Channel { get; private set; }
        public string SourceName { get; private set; }
        public SoundState CurrentState => _state;

        public event Action<ISoundSource> OnStopped;
        public event Action<ISoundSource> OnFinished;
        public event Action<ISoundSource> OnLoopFinished;
        public event Action<ISoundSource> OnPaused;
        public event Action<ISoundSource> OnFadeInComplete;
        public event Action<ISoundSource> OnFadeOutComplete;
        
        #endregion
        
        #region IPoolable Actions

        public event Action<IPoolableSound> OnRelease;

        #endregion

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _fadeVolume = 1;
            _userVolume = 1;
            _contextVolume = 1;
        }

        private void SetAudioSourceValues()
        {
            if (_audioSource == null) return;
            if (_soundSourceData == null) return;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            gameObject.name = _soundSourceData.SoundName;
            SourceName = _soundSourceData.SoundName;
#endif
            var soundData = _soundSourceData;
            
            // === Base Data === //
            var baseData = soundData.BaseData;
            _audioSource.panStereo = baseData.StereoPan;
            _audioSource.reverbZoneMix = baseData.ReverbZoneMix;
            
            // === Priority Data === //
            var priorityData = soundData.PriorityData;
            _audioSource.priority = priorityData.Priority;
            
            // === Flags Data === //
            var flagsData = soundData.AudioFlags;
            _audioSource.bypassEffects = flagsData.IgnoreEffects;
            _audioSource.bypassListenerEffects = flagsData.IgnoreListenerEffects;
            _audioSource.bypassReverbZones = flagsData.IgnoreReverbZones;
            
            // === Channel Data === //
            var channelData = soundData.ChannelData;
            _audioSource.outputAudioMixerGroup = channelData.MixerGroup;
            Channel = channelData.Channel;
            
            // === Spatial Data === //
            var spatialData = soundData.SpatialData;
            _doesMove = spatialData.DoesMove;
            _audioSource.spatialBlend = spatialData.SpatialBlend;
            _audioSource.rolloffMode = spatialData.RolloffMode;

            if (spatialData.RolloffMode == AudioRolloffMode.Custom)
            {
                if (spatialData.RolloffCustomCurve is { length: > 0 })
                {
                    _audioSource.SetCustomCurve(
                        AudioSourceCurveType.CustomRolloff, 
                        spatialData.RolloffCustomCurve);
                }
            }

            _audioSource.minDistance = spatialData.MinDistance;
            _audioSource.maxDistance = spatialData.MaxDistance;
            _audioSource.dopplerLevel = spatialData.DopplerLevel;
            _audioSource.spread = spatialData.Spread;
            _audioSource.spatialize = spatialData.Spatialize;
            
            // === Playback Data === //
            var playbackData = soundData.PlaybackData;
            DoesLoop = playbackData.DoesLoop;
            _doesFadeOut = playbackData.DoesFadeOut;
            _doesFadeIn = playbackData.DoesFadeIn;
            _audioSource.loop = playbackData.DoesLoop;
            _audioSource.volume = playbackData.Volume;
            _audioSource.pitch = playbackData.Pitch;
        }

        private void ClearAudioSourceValues()
        {
            if (_audioSource == null) return;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            gameObject.name = "AudioSource";
            SourceName = string.Empty;
#endif

            // === Base defaults ===
            _audioSource.panStereo = 0f;
            _audioSource.reverbZoneMix = 1f;
            
            // === Priority Data === //
            _audioSource.priority = 128;

            // === Flags defaults ===
            _audioSource.bypassEffects = false;
            _audioSource.bypassListenerEffects = false;
            _audioSource.bypassReverbZones = false;

            // === Channel defaults ===
            _audioSource.outputAudioMixerGroup = null;
            Channel = default;

            // === Spatial defaults ===
            _audioSource.spatialBlend = 0f;               // 2D
            _audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            _audioSource.minDistance = 1f;
            _audioSource.maxDistance = 500f;
            _audioSource.dopplerLevel = 1f;
            _audioSource.spread = 0f;
            _audioSource.spatialize = false;
            _doesMove = false;

            // === Playback defaults ===
            DoesLoop = false;
            _doesFadeOut = false;   
            _doesFadeIn = false;   
            _audioSource.loop = false;
            _audioSource.volume = 1f;
            _audioSource.pitch = 1f;

            // === Extra safety ===
            if (_audioSource.clip != null)
            {
                _audioSource.time = 0f;
                _audioSource.clip = null;
            }
        }
        
        public void UpdateVolume()
        {
            _audioSource.volume = _soundSourceData.PlaybackData.Volume * GetVolume();
        }

        private void SetContextVolume(float value)
        {
            _contextVolume = Mathf.Clamp(value, MinAudible, 1);
        }
        
        private void SetFadeVolume(float value)
        {
            _fadeVolume = Mathf.Clamp(value, MinAudible, 1);
        }

        #region IPoolable Methods
        
        public void OnGetFromPool()
        {
            gameObject.SetActive(true);
        }

        public void OnReleasedFromPool()
        {
            Stop();
            ClearAudioSourceValues();
            transform.parent = null;
            gameObject.SetActive(false);
        }

        public void OnDestroyFromPool()
        {
            Destroy(gameObject);
        }

        public void ReleaseFromPool()
        {
            OnRelease?.Invoke(this);
        }

        #endregion

        #region ISoundSource Methods

        #region Getters
        public float GetVolume() => Mathf.Clamp01(_fadeVolume * _userVolume * _contextVolume);
        public bool GetShouldFadeOut()
        {
            if(_audioSource == null) return false;
            if(_audioSource.clip == null) return false;
            
            return _doesFadeOut && _audioSource.time > (GetAudioLenght() - _soundSourceData.PlaybackData.FadeOutDuration);
        }

        public float GetPlayRatio()
        {
            if(_audioSource == null) return 1f;
            if(_audioSource.clip == null) return 1f;
            
            return Mathf.Clamp01(_audioSource.time / GetAudioLenght());
        }

        public float GetAudioLenght()
        {
            if(_audioSource == null) return Mathf.Infinity;
            if(_audioSource.clip == null) return Mathf.Infinity;
            
            return _audioSource.clip.length;
        }

        #endregion
        
        public void SetVolume(float volume)
        {
            _userVolume = Mathf.Clamp(volume, MinAudible, 1);
            UpdateVolume();
        }
        
        public void SetSoundData(ISoundSourceData soundSourceData, Transform parent = null)
        {
            _soundSourceData = soundSourceData;
            
            _state = SoundState.Stopped;
            SetAudioSourceValues();
            
            if (_soundSourceData.Is3DSound)
            {
                if (parent == null)
                {
                    Debug.Log("Parent is null");
                    return;
                }

                if (_doesMove)
                {
                    transform.SetParent(parent);
                    transform.localPosition = Vector3.zero;
                }
                else
                {
                    transform.position = parent.position;
                }
                
            }
        }

        #region Audio Actions
        
        public void Play()
        {
            if(_state != SoundState.Stopped) return;
            
            _audioSource.clip = _soundSourceData.ClipsData.GetAudioClip();
            
            if (_soundSourceData.PlaybackData.DoesFadeIn)
            {
                SetFadeVolume(0f);
                _audioSource.Play();
                FadeIn(SetFadeVolume, _fadeVolume, 1,
                    onStart: () => _state = SoundState.Fading,
                    onComplete: () => _state = SoundState.Foreground);
            }
            else
            {
                _state = SoundState.Foreground;
                _audioSource.Play();
            }
        }
        public void Stop()
        {
            if (_state != SoundState.Foreground)
            {
                if (_state == SoundState.Paused)
                {
                    _state = SoundState.Stopped;
                    OnStopped?.Invoke(this);
                }

                return;
            }
            
            if (_soundSourceData.PlaybackData.DoesFadeOut)
            {
                FadeOut(SetFadeVolume, _fadeVolume,0,
                    onStart: () => _state = SoundState.Fading,
                    onComplete: () =>
                    {
                        _audioSource.Stop();
                        _state = SoundState.Stopped;
                        OnStopped?.Invoke(this);
  
                    }
                );
            }
            else
            {
                _state = SoundState.Stopped;
                _audioSource.Stop();
                OnStopped?.Invoke(this);
            }
        }
        
        public void Resume()
        {
            if(_state != SoundState.Paused) return;
            
            if (_soundSourceData.PlaybackData.DoesFadeIn)
            {
                SetFadeVolume(0f);
                FadeIn(SetFadeVolume,_fadeVolume ,1,
                    onStart: () =>
                    {
                        _state = SoundState.Fading;
                        _audioSource.Play();
                    },
                    onComplete: () => _state = SoundState.Foreground);
            }
            else
            {
                _state = SoundState.Foreground;
                _audioSource.Play();
            }
        }
        
        public void Pause()
        {
            if(_state != SoundState.Foreground) return;
            
            Internal_Pause(OnPaused);
        }

        public void FinishLoop()
        {
            if(_state != SoundState.Foreground) return;
            
            Internal_Pause(OnLoopFinished);
        }

        public void Internal_Pause(Action<ISoundSource> onEndAction)
        {
            if (_soundSourceData.PlaybackData.DoesFadeOut)
            {
                FadeOut(SetFadeVolume,_fadeVolume, 0,
                    onStart: () => _state = SoundState.Fading,
                    onComplete: () =>
                    {
                        _audioSource.Pause();
                        _state = SoundState.Paused;
                        onEndAction?.Invoke(this);
                    }
                );
            }
            else
            {
                _state = SoundState.Paused;
                _audioSource.Pause();
                onEndAction?.Invoke(this);
            }
        }
        
        
        public void SetAsBackground(float volumeTarget = 0.5f)
        {
            if (_state == SoundState.Foreground)
            {

                FadeOut(SetContextVolume,_contextVolume, volumeTarget ,onStart: null, onComplete: () => _state = SoundState.Background );
            }
        }
        
        public void SetAsForeground()
        {
            if (_state == SoundState.Background)
            {
                FadeIn(SetContextVolume, _contextVolume, 1f,onStart: null, onComplete: () => _state = SoundState.Foreground);
            }
        }

        public void FinishSound()
        {
            _state = SoundState.Stopped;
            OnFinished?.Invoke(this);
            ReleaseFromPool();
        }

        public void DetachFromParent()
        {
            if(_doesMove == false) return;
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            SoundTools.DebugSound($"Sound {SourceName} has been detached from its parent!");
#endif
            
            transform.SetParent(null);
        }

        #endregion
        
        #endregion

        #region Fade 
        
        private void FadeIn(Action<float> volumeToChange, float startValue = 0 , float targetValue = 1, Action onStart = null,
            Action onComplete = null)
        {
            if(_state == SoundState.Fading) return;
            
            StartFade(Coroutine_FadeIn(
                targetValue,startValue ,volumeToChange, onStart, () => onComplete?.Invoke()));
        }
        private void FadeOut(Action<float> volumeToChange, float startValue = 0,  float targetValue = 0, Action onStart = null,
            Action onComplete = null)
        {
            if(_state == SoundState.Fading) return;
            
            StartFade(Coroutine_FadeOut(
                targetValue, startValue,volumeToChange, onStart, () => onComplete?.Invoke()));
        }

        private void StartFade(IEnumerator routine)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(routine);
        }

        private IEnumerator Coroutine_FadeIn(
            float targetValue,
            float startValue,
            Action<float> volumeToChange,
            Action onStart,
            Action onComplete)
        {
            float duration = _soundSourceData.PlaybackData.FadeInDuration;
            var elapsedTime = 0f;
            
            onStart?.Invoke();
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float ratio = Mathf.Clamp01(elapsedTime / duration);

                float perceptual = Mathf.Lerp(startValue, targetValue, ratio * ratio);
                
                
                volumeToChange.Invoke(perceptual);
                UpdateVolume();
                yield return null;
            }
            
            volumeToChange.Invoke(targetValue);
            UpdateVolume();
            onComplete?.Invoke();
            OnFadeInComplete?.Invoke(this);
        }
        
        private IEnumerator Coroutine_FadeOut(
            float targetValue,
            float startValue,
            Action<float> volumeToChange,
            Action onStart,
            Action onComplete)
        {
            float duration = _soundSourceData.PlaybackData.FadeOutDuration;
            var elapsedTime = 0f;
            
            targetValue = Mathf.Clamp(targetValue, MinAudible, 1);
            
            onStart?.Invoke();
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float ratio = Mathf.Clamp01(elapsedTime / duration);

                float perceptual = Mathf.Lerp(startValue, targetValue, ratio * ratio);
                
                
                volumeToChange.Invoke(perceptual);
                UpdateVolume();
                yield return null;
            }
            
            volumeToChange.Invoke(targetValue);
            UpdateVolume();
            onComplete?.Invoke();
            OnFadeOutComplete?.Invoke(this);
        }
        
        
        #endregion
    }
}