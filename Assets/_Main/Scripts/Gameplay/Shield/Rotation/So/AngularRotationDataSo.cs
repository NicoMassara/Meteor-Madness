using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.So
{
    [CreateAssetMenu(fileName = "So_AngularRotation_Default", menuName = "Shield Rotation/Angular Rotation Data", order = 0)]
    public class AngularRotationDataSo : ScriptableObject, IAngularRotationData
    {
        [Header("Speed Values")]
        [Min(1)]
        [SerializeField] private float acceleration = 1200;
        [Min(1)]
        [SerializeField] private float maxSpeed = 600;
        
        [Space]
        
        [Header("Min Values")]
        [Range(0,45f)]
        [SerializeField] private float minInputAngle;
        
        [Space]
        
        [Header("Magnitude Curve")]
        [SerializeField] private AnimationCurve magnitudeCurve;

        public float Acceleration => acceleration;
        public float MaxSpeed => maxSpeed;
        public float MinInputAngle => minInputAngle;
        public AnimationCurve MagnitudeCurve => magnitudeCurve;
    }
}