using UnityEngine;

namespace _Main.Scripts.Contracts.Interfaces
{
    public struct ShakeData
    {
        public IShakerData Data;
        public Vector2 Direction;
        public float DirectionBias;
        public float Multiplier;
    }
    
    public interface IShakerData
    {
        public float Magnitude { get; }
        public float Duration { get; }
        public float Frequency { get; }
        AnimationCurve IntensityCurve { get; }
        public int Priority { get;}
        public bool DoesCancelAll { get; }
    }
}