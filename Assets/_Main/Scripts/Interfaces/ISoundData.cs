using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Interfaces
{
    public interface ISoundData
    {
        public AudioSourceData SourceData { get; }
        public FadeData FadeInData { get; }
        public FadeData FadeOutData { get; }
        public SoundChannel Channel { get; }
        public bool Is3DSound { get; }
        public string ClassName { get; }
        public bool IsUniqueClip { get; }
        public bool DoesLoop { get; }
        public bool HasRandomPitch { get; }
        public AudioClip GetAudioClip();
        public float GetRandomPitch();
    }
}