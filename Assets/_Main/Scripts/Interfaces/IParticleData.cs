using UnityEngine;

namespace _Main.Scripts.Interfaces
{
    public interface IParticleData
    {
        public Sprite Sprite { get; }

        public Vector3 PositionOffset { get; }

        public float RotationOffset { get; }

        public float MoveSpeed { get; }

        public float TimeToReachScale { get; }

        public Vector3 TargetScale { get; }

        public Vector3 StartScale { get; }

        public float RatioTimeToStartFade { get; }

        public float TimeToFade { get; }
    }
}