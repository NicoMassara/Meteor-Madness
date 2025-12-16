using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Plugins.NicolasMassara.CustomSoundManager
{
    [CreateAssetMenu(fileName = "So_SoundSourceData_NAME", menuName = "Sound Manager/Source/Sound Data", order = 0)]
    
    public class SoundSourceDataSo : ScriptableObject, ISoundSourceData
    {
        #region Private Classes
        
        [Serializable]
        private class SourceClipsData : ISourceClipsData
        {
            [Header("Audio Clips Data")]
            [SerializeField] private AudioClip[] clips;
            [Tooltip("Only works when there is multiple audio clips, if is off it'll play in random order.")]
            [SerializeField] private bool doesPlayMultipleInOrder = true;
            
            private int _lastClipIndex;
            private int _currentClipIndex;
            
            public AudioClip GetAudioClip()
            {
                if (clips.Length == 0)
                {
                    Debug.LogWarning($"No Clips Added in SoundComponentDataSo");
                    return null;
                }

                if (GetClipsMaxIndex() == 0)
                {
                    return clips[0];
                }

                return doesPlayMultipleInOrder ?  
                    GetNextAudioClip() :
                    GetRandomAudioClip();
            }
            
            private int GetClipsMaxIndex()
            {
                return clips.Length-1;
            }

            private AudioClip GetRandomAudioClip()
            {
                var index = SoundTools.GetRandomIndex(GetClipsMaxIndex(), ref _lastClipIndex);
                return clips[index];
            }

            private AudioClip GetNextAudioClip()
            {
                SoundTools.IncrementIndex(ref _currentClipIndex, GetClipsMaxIndex());
                return clips[_currentClipIndex];
            }
        }

        [Serializable]
        private class AudioPriorityData : ISoundPriorityData
        {
            [Range(0, 256)]
            [SerializeField] private int priority = 128;
            public int Priority => priority;
        }

        [Serializable]
        private class AudioSourceBaseData : IAudioSourceBaseData
        {
            [Range(-1, 1)]
            [SerializeField] private float stereoPan = 0f;
            [Range(0, 1)]
            [SerializeField] private float reverbZoneMix = 1f;
            
            public float StereoPan => stereoPan;
            public float ReverbZoneMix => reverbZoneMix;
        }
        
        [Serializable]
        private class AudioSourceFlagsData : IAudioSourceFlagsData
        {
            [SerializeField] private bool ignoreEffects;
            [SerializeField] private bool ignoreListenerEffects;
            [SerializeField] private bool ignoreReverbZones;

            public bool IgnoreEffects => ignoreEffects;
            public bool IgnoreListenerEffects => ignoreListenerEffects;
            public bool IgnoreReverbZones => ignoreReverbZones;
        }

        [Serializable]
        private class AudioChannelData : IAudioChannelData
        {
            [SerializeField] private AudioMixerGroup mixerGroup;
            [Tooltip("Different from MixerGroup, is used to track the amount of sounds from the selected channel.")]
            [SerializeField] private SoundChannel channel;

            public AudioMixerGroup MixerGroup => mixerGroup;
            public SoundChannel Channel => channel;

            public void ForceChannel(SoundChannel forcedChannel)
            {
                this.channel = forcedChannel;
            }
        }
        
        [Serializable]
        private class SpatialAudioSourceData : ISpatialAudioSourceData
        {
            [Header("Spatial")]
            [SerializeField, Range(0f, 1f)] private float spatialBlend = 1f;

            [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

            [Header("Distance")]
            [SerializeField, Min(0f)] private float minDistance = 1f;

            [SerializeField, Min(0f)] private float maxDistance = 30f;

            [Header("Effects")]
            [SerializeField, Range(0f, 5f)] private float dopplerLevel = 0f;

            [SerializeField, Range(0f, 360f)]
            private float spread = 0f;

            [SerializeField] private bool spatialize = false;
            [SerializeField] private bool doesMove = false;

            public float SpatialBlend => spatialBlend;
            public AudioRolloffMode RolloffMode => rolloffMode;
            public float MinDistance => minDistance;
            public float MaxDistance => maxDistance;
            public float DopplerLevel => dopplerLevel;
            public float Spread => spread;
            public bool Spatialize => spatialize;

            public bool DoesMove => doesMove;
        }
        
        [Serializable]
        private class AudioPlaybackData : IAudioPlaybackData
        {
            [Header("Playback")]
            [SerializeField] private bool doesLoop;
            [SerializeField] private float intervalBetweenLoops = 1f;
            [Range(0, 2)]
            [SerializeField] private float volume = 1f;
            [Space()]
            [Header("Pitch")]
            [Range(-3, 3)]
            [SerializeField] private float pitch = 1f;
            [Range(0,1)]
            [SerializeField] private float randomPitchRange;
            [Space()]
            [Header("Fade")]
            [SerializeField] private float fadeInDuration = 1f;
            [SerializeField] private float fadeOutDuration = 1f;

            public bool DoesLoop => doesLoop;
            public float IntervalBetweenLoops => intervalBetweenLoops;

            public float Volume => volume;
            public float Pitch => randomPitchRange > 0 ? 
                pitch + SoundTools.GetRandomPitch(randomPitchRange) : pitch;

            public bool DoesFadeIn => fadeInDuration > 0f;
            public float FadeInDuration => fadeInDuration;

            public bool DoesFadeOut => fadeOutDuration > 0f;
            public float FadeOutDuration => fadeOutDuration;
        }
        

        #endregion

        #region Public Serializers
        
        [Header("Name")]
        [SerializeField] private string soundName;
        [Space(2)]
        [Header("Channel Settings")]
        [SerializeField] private SourceClipsData clipsData;
        [Space]
        [Header("Channel Settings")]
        [SerializeField] private AudioChannelData channelSettings;
        [Space]
        [Header("Sound Properties")]
        [SerializeField] private AudioSourceFlagsData audioFlags;
        [Space]
        [Header("Priority Properties")]
        [SerializeField] private AudioPriorityData priorityData;
        [Space]
        [Header("Sound Settings")]
        [SerializeField] private AudioSourceBaseData baseData;
        [Space]
        [Header("Playback Settings")]
        [SerializeField] private AudioPlaybackData audioPlaybackData;
        [Space]
        [Header("Spatial Settings")]
        [SerializeField] private bool is3dSound;
        [SerializeField] private SpatialAudioSourceData spatialData;

        
        #endregion

        #region ISoundComponentData

        public string SoundName => soundName;
        public ISourceClipsData ClipsData => clipsData;
        public IAudioChannelData ChannelData => channelSettings;
        public IAudioSourceFlagsData AudioFlags => audioFlags;
        public ISoundPriorityData PriorityData => priorityData;
            
        public IAudioSourceBaseData BaseData => baseData;
        public IAudioPlaybackData PlaybackData => audioPlaybackData;
        public bool Is3DSound => is3dSound;
        public ISpatialAudioSourceData SpatialData => spatialData;
        
        #endregion

        protected void ForceChannel(SoundChannel channel)
        {
            channelSettings.ForceChannel(channel);
        }

    }
}