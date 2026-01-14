using MeteorMadness.GlobalValues.Tools;
using UnityEngine;

namespace _Main.Scripts.Core.FlyingObject.Components
{
    [CreateAssetMenu(fileName = "So_FlyingObjectTrail_Default", menuName = "FlyingObject/Trail Data", order = 0)]
    public class FlyingObjectTrailDataSo : ScriptableObject
    {
        [System.Serializable]
        public class TrailOscillatorData
        {
            [Min(0)] public float speed = 50;
            [Min(0)] public float amplitude = 0.12f;
            [Min(0)] public float offset = 1;
        }

        [SerializeField] private TrailOscillatorData trailScaleData;
        [SerializeField] private TrailOscillatorData trailRotateData;

        public TrailOscillatorData TrailScaleData => trailScaleData;
        public TrailOscillatorData TrailRotateData => trailRotateData;
    }
}