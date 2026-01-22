using UnityEngine;

namespace _Main.Scripts.Movement
{
    [CreateAssetMenu(fileName = "So_MovementData_Default", menuName = "Movement Rotation/Data", order = 0)]
    public class MovementDataSo : ScriptableObject, IMovementData
    {
        [Header("Speeding Values")]
        [Min(1)]
        [SerializeField] private float acceleration = 1200;
        [Min(1)]
        [SerializeField] private float deAcceleration = 600;
        [Min(1)]
        [SerializeField] private float maxSpeed = 600;
        
        [Space]
        [Header("Min Values")]
        [Range(0,45f)]
        [SerializeField] private float minInputAngle;
        [Range(0, 1)] 
        [SerializeField] private float maxTravelDistanceRatioToDeAccelerate = 0.85f;
        [Range(0, 1)] 
        [SerializeField] private float minDistanceRatioToSnap = 0.95f;
        
        [Space]
        [Header("Magnitude Curve")]
        [SerializeField] private AnimationCurve magnitudeCurve;

        public float Acceleration => acceleration;
        public float DeAcceleration => deAcceleration;
        public float MaxSpeed => maxSpeed;
        public float MinInputAngle => minInputAngle;
        public float MaxTravelDistanceRatioToDeAccelerate => maxTravelDistanceRatioToDeAccelerate;
        public float MinDistanceRatioToSnap => minDistanceRatioToSnap;
        public AnimationCurve MagnitudeCurve => magnitudeCurve;
    }
}