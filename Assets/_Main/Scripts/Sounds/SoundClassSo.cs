using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Sounds
{
    [CreateAssetMenu(fileName = "SO_SoundData_Sound", menuName = "Scriptable Objects/Sound/Sound Class", order = 0)]
    public class SoundClassSo : ScriptableObject
    {
        [SerializeField] private string className = "SoundClass";
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private AudioSourceData sourceData;
        [Range(0,1)]
        [SerializeField] private float randomPitchRange;
        private int _lastIndex;
        
        public AudioSourceData SourceData => sourceData;
        public string ClassName => className;
        public bool IsUniqueClip => clips.Length == 1;

        public bool HasRandomPitch => randomPitchRange > 0;

        public AudioClip GetAudioClip()
        {
            return GetRandomClip();
        }

        private AudioClip GetRandomClip()
        {
            var count = clips.Length;
            var index = 0;

            if (count > 1)
            {
                index = Random.Range(0, count);
                
                if (index == _lastIndex)
                {
                    index++;
                    if (index == count)
                    {
                        index = 0;
                    }
                }
            }
            
            _lastIndex = index;
            
            return clips[index];
        }

        public float GetRandomPitch()
        {
            return 1 + Random.Range(-randomPitchRange, randomPitchRange);
        }
    }

    
    [Serializable]
    public class AudioSourceData
    {
        public AudioMixerGroup mixerGroup;
        public bool ignoreEffects;
        public bool ignoreListenerEffects;
        public bool ignoreReverbZones;
        public bool loop;
        [Range(0, 256)] 
        public int priority = 128;
        [Range(0, 2)] 
        public float volume = 1;
        [Range(-3, 3)] 
        public float pitch = 1;
        [Range(-1, 1)] 
        public float stereoPan = 0;
        [Range(0, 1)] 
        public float spatialBlend = 0;
        [Range(0, 1.1f)] 
        public float reverbZoneMix = 1;
        [Range(0,1f)]
        public float dopplerLevel = 0;
    }
}