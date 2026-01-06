using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Movement
{
    public interface IMovementData
    {
        public float MaxSpeed { get; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public float SnapSpeed { get; }
        public float CorrectionSnapSpeed { get; }
        public float ChangeDirectionAcceleration { get; }
    }
    
    
    public interface IFsmMovementData
    {
        public float StopThreshold { get; }
        public float MinSpeedRatioToSnap { get; }
        public float StopChangeDirectionSpeedRatio { get; }
    }

    [CreateAssetMenu(fileName = "So_ShieldRotation_Movement_Default", menuName = "Shield Rotation/Movement Data", order = 0)]
    public class MovementDataSo : ScriptableObject, 
        IMovementData, IFsmMovementData
    {
        [Header("Speed")]
        [Min(1)]
        [SerializeField] private float maxSpeed;
        [Header("Acceleration")]
        [Min(1)]
        [SerializeField] private float acceleration;
        [Min(1)]
        [SerializeField] private float deceleration;
        [Min(1)]
        [SerializeField] private float changeDirectionAcceleration;
        [Header("Snap Values")]
        [Range(0,1)] 
        [SerializeField] private float minSpeedRatioToSnap;
        [Min(1)]
        [SerializeField] private float snapSpeed;
        [Min(1)]
        [SerializeField] private float correctionSnapSpeed;
        [Header("Misc")]
        [SerializeField] private float stopThreshold;
        [Range(0,1)]
        [SerializeField] private float stopChangeDirectionSpeedRatio;

        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float Deceleration => deceleration;
        public float ChangeDirectionAcceleration => changeDirectionAcceleration;
        public float MinSpeedRatioToSnap => minSpeedRatioToSnap;
        public float CorrectionSnapSpeed => correctionSnapSpeed;
        public float SnapSpeed => snapSpeed;
        public float StopThreshold => stopThreshold;
        public float StopChangeDirectionSpeedRatio => stopChangeDirectionSpeedRatio;
    }
}