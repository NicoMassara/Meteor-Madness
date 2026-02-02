using UnityEngine;

namespace _Main.Scripts.Common
{
    public interface IShakerCap
    {
        public float MaxOffset { get; }
        public bool SmoothCap { get; }
        public float SoftCapStrength { get; }
    }

    [CreateAssetMenu(fileName = "SO_ShakerCap_Default", menuName = "Scriptable Objects/Shaker/Cap Data", order = 0)]
    public class ShakerCapDataSo : ScriptableObject,IShakerCap
    {
        [SerializeField] private bool smoothCap = true;
        [Min(0.01f)]
        [SerializeField] private float maxOffset = 0.6f;
        [Min(0.1f)]
        [SerializeField] private float softCapStrength = 1.5f;

        public bool SmoothCap => smoothCap;
        public float MaxOffset => maxOffset;
        public float SoftCapStrength => softCapStrength;
    }
}