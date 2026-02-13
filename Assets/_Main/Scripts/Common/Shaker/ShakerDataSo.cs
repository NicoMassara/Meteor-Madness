using _Main.Scripts.Contracts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Common
{
    [System.Serializable]
    public class DirectionalShakeData
    {
        public ShakerDataSo Data;
        [Range(0, 1)] 
        public float DirectionBias = 0.25f;
    }

    [CreateAssetMenu(fileName = "SO_ShakerData_Default", menuName = "Scriptable Objects/Shaker/Shaker Data", order = 0)]
    public class ShakerDataSo : ScriptableObject, IShakerData
    {
        [Min(0.02f)]
        [SerializeField] private float magnitude;
        [Min(0.06f)]
        [SerializeField] private float duration;
        [Min(8)]
        [SerializeField] private float frequency;
        [SerializeField] private AnimationCurve intensityCurve;
        [Min(0)]
        [SerializeField] private int priority;
        [SerializeField] private bool doesCancelAll;

        public float Magnitude => magnitude;
        public float Duration => duration;
        public float Frequency => frequency;
        public AnimationCurve IntensityCurve => intensityCurve;
        public int Priority => priority;
        public bool DoesCancelAll => doesCancelAll;
    }
}