using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Plugins.NicolasMassara.CustomSoundManager
{
    public abstract class SoundTools
    {
        private const string SoundPrefabPath = "Sounds/SoundSourcePrefab";
        private const string AudioMixerPath = "Sounds/MainAudioMixer";

        private const bool DoesDebug = true;

        public static void DebugSound(string debugString)
        {
#pragma warning disable CS0162 // Unreachable code detected
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            
            if(DoesDebug)
                Debug.Log(debugString);
            
#endif
#pragma warning restore CS0162 // Unreachable code detected
        }

        public static float GetRandomPitch(float pitchModifier)
        {
            return 1 + UnityEngine.Random.Range(-pitchModifier, pitchModifier);
        }

        public static int GetRandomIndex(int maxIndex, ref int lastIndex)
        {
            if (lastIndex < 0) throw new ArgumentOutOfRangeException(nameof(lastIndex));

            var selectedIndex = 0;

            if (maxIndex > 1)
            {
                selectedIndex = Random.Range(0, maxIndex);

                if (selectedIndex == lastIndex)
                {
                    selectedIndex++;
                    selectedIndex = Math.Clamp(selectedIndex, 0, maxIndex);
                }
            }

            lastIndex = selectedIndex;
            return selectedIndex;
        }

        public static void IncrementIndex(ref int currentIndex, int maxIndex)
        {
            currentIndex++;
            currentIndex = Math.Clamp(currentIndex, 0, maxIndex);
        }

        public static int GetSoundChannelLimit(SoundChannel channel)
        {
            return channel switch
            {
                SoundChannel.Music => 2,
                SoundChannel.Sfx => 5,
                SoundChannel.UI => 3,
                SoundChannel.Collision => 3,
                SoundChannel.Meteor => 2,
                SoundChannel.Deflection => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null)
            };
        }

        public static float GetDbFrom01Value(float value)
        {
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        }

        public static SoundSource GetSoundSourcePrefab()
        {
            var prefab = Resources.Load<GameObject>(SoundPrefabPath);
            return prefab != null ? prefab.GetComponent<SoundSource>() : null;
        }

        public static AudioMixer GetAudioMixer() => Resources.Load<AudioMixer>(AudioMixerPath);
        
    }

    public class AudioMixerTools
    {
        private const string MixerVolumeParameterName = "MasterVolume";
        private const string MusicVolumeParameterName = "MusicVolume";
        private const string GameplayVolumeParameterName = "GameplayVolume";
        private const string MeteorVolumeParameterName = "MeteorVolume";
        private const string ShieldVolumeParameterName = "ShieldVolume";
        private const string EarthVolumeParameterName = "EarthVolume";
        private const string SfxVolumeParameterName = "SfxVolume";
        private const string UIVolumeParameterName = "UIVolume";
        
        public static void SetMixerChannelVolume(MixerChannels mixerChannel, float volume)
        {
            string channelName = mixerChannel switch
            {
                MixerChannels.Master => MixerVolumeParameterName,
                MixerChannels.Music => MusicVolumeParameterName,
                MixerChannels.Sfx => SfxVolumeParameterName,
                MixerChannels.UI => UIVolumeParameterName,
                MixerChannels.Gameplay => GameplayVolumeParameterName,
                MixerChannels.Meteor => MeteorVolumeParameterName,
                MixerChannels.Shield => ShieldVolumeParameterName,
                MixerChannels.Earth => EarthVolumeParameterName,
                _ => throw new ArgumentOutOfRangeException(nameof(mixerChannel), mixerChannel, null)
            };

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"Set {mixerChannel} Mixer Channel Volume: {volume}");
#endif
            
            SoundManager.SetMainVolume(channelName, volume);
        }
    }
}