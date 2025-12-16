using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Plugins.NicolasMassara.CustomSoundManager
{
    #region Data
    
    public interface ISourceClipsData
    {
        public AudioClip GetAudioClip();
    }
    
    public interface ISoundPriorityData
    {
        public int Priority { get; }
    }

    public interface IAudioSourceBaseData
    {
        public float StereoPan { get; }
        public float ReverbZoneMix { get; }
    }
    
    public interface IAudioSourceFlagsData
    {
        public bool IgnoreEffects { get; }
        public bool IgnoreListenerEffects { get; }
        public bool IgnoreReverbZones { get; }
    }

    
    public interface IAudioChannelData
    {
        public AudioMixerGroup MixerGroup { get; }
        public SoundChannel Channel { get; }
    }
    
    public interface ISpatialAudioSourceData
    {
        public bool DoesMove { get; }
        public float SpatialBlend { get; }
        public AudioRolloffMode RolloffMode { get; }
        public float MinDistance { get; }
        public float MaxDistance { get; }
        public float DopplerLevel { get; }
        public float Spread { get; }
        public bool Spatialize { get; }
    }
    
    public interface IAudioPlaybackData
    {
        public bool DoesLoop { get; }
        public float IntervalBetweenLoops { get; }

        public float Volume { get; }
        public float Pitch { get; }

        public bool DoesFadeIn { get; }
        public float FadeInDuration { get; }

        public bool DoesFadeOut { get; }
        public float FadeOutDuration { get; }
    }
    
    public interface ISoundSourceData
    {
        public string SoundName { get; }
        public ISourceClipsData ClipsData { get; }
        public IAudioChannelData ChannelData { get; }
        public IAudioSourceFlagsData AudioFlags { get; }
        public IAudioSourceBaseData BaseData { get; }
        public ISoundPriorityData PriorityData { get; }
        
        public bool Is3DSound { get; }
        public IAudioPlaybackData PlaybackData { get; }
        public ISpatialAudioSourceData SpatialData { get; }
    }
    
    #endregion

    #region Sound Source
    
    public interface IPoolableSound : IPoolable<IPoolableSound> { }
    public interface ISoundSource
    {
        public event Action<ISoundSource> OnStopped;
        public event Action<ISoundSource> OnFinished;
        public event Action<ISoundSource> OnPaused;
        public event Action<ISoundSource> OnLoopFinished;
        
        public event Action<ISoundSource> OnFadeInComplete;
        public event Action<ISoundSource> OnFadeOutComplete;
        
        public SoundChannel Channel { get; }
        public string SourceName { get; }
        public SoundState CurrentState { get; }
        public void SetVolume(float volume);
        public bool DoesLoop { get; }
        public float GetVolume();
        public float GetPlayRatio();
        public float GetAudioLenght();
        public bool GetShouldFadeOut();
        public void SetSoundData(ISoundSourceData soundSourceData,Transform parent);
        public void Play();
        public void Stop();
        public void Resume();
        public void Pause();
        public void FinishLoop();
        public void SetAsBackground(float volumeTarget);
        public void SetAsForeground();
        public void FinishSound();
    }
    
    #endregion
}