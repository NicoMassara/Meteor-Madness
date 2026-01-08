using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Movement
{
    public interface IMovementData
    {
        public float MaxSpeed { get; }
        public float Acceleration { get; }
        public float SnapSpeed { get; }
        public float CorrectionSnapSpeed { get; }
        public float ChangeDirectionAcceleration { get; }
        public int MaxSlotTravelDistance { get; }
        public Vector2Int SlotRangeToSnap { get; }
    }
    
    
    public interface IFsmMovementData
    {
        public float StopThreshold { get; }
        public float StopChangeDirectionSpeedRatio { get; }
        public float ChangeDirectionSpeedRatio { get; }
        public float ChangeDirectionTimeOut { get; }
        public float StopChangeDirectionTimeOut { get; }
        public float ChangeDirectionTimeOutThreshold { get; }
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
        [Space]
        [Header("Stop")]
        [Tooltip("Time to start stop after input release")]
        [Min(0)]
        [SerializeField] private float stopThreshold;
        [Space]
        [Header("Snap")]
        [SerializeField] private SnapRange snapRange;
        
        [System.Serializable]
        private class SnapRange
        {
            [Min(0)]
            [SerializeField] private int minSlotToSnap;
            [Min(1)]
            [SerializeField] private int maxSlotToSnap;
            
            public Vector2Int Range => new (minSlotToSnap, maxSlotToSnap);
        }
        
        [Min(1)]
        [SerializeField] private int maxSlotTravelDistance;
        [Min(1)]
        [SerializeField] private float snapSpeed;
        [Min(1)]
        [SerializeField] private float correctionSnapSpeed;
        [Space]
        [Header("Change Direction")]
        [Min(1)]
        [SerializeField] private float changeDirectionAcceleration;
        [Range(0,1)]
        [SerializeField] private float changeDirectionSpeedRatio;
        [Range(0,1)]
        [SerializeField] private float stopChangeDirectionSpeedRatio;
        [Range(0,1)]
        [SerializeField] private float changeDirectionTimeOutThreshold;

        public float MaxSpeed => maxSpeed;
        public float Acceleration => acceleration;
        public float ChangeDirectionAcceleration => changeDirectionAcceleration;
        public float CorrectionSnapSpeed => correctionSnapSpeed;
        public float SnapSpeed => snapSpeed;
        public float StopThreshold => stopThreshold;
        public float ChangeDirectionSpeedRatio => changeDirectionSpeedRatio;
        public float StopChangeDirectionSpeedRatio => stopChangeDirectionSpeedRatio;
        public float ChangeDirectionTimeOut => GetChangeDirectionTimeOut();
        public float StopChangeDirectionTimeOut => GetStopChangeDirectionTimeOut();
        public float ChangeDirectionTimeOutThreshold => changeDirectionTimeOutThreshold + 1;
        public int MaxSlotTravelDistance => maxSlotTravelDistance;
        public Vector2Int SlotRangeToSnap => snapRange.Range;

        private float GetChangeDirectionTimeOut()
        {
            var targetSpeed = MaxSpeed * ChangeDirectionSpeedRatio;
            return (MaxSpeed - targetSpeed) / ChangeDirectionAcceleration;
        }
        
        private float GetStopChangeDirectionTimeOut()
        {
            var targetSpeed = MaxSpeed * StopChangeDirectionSpeedRatio;
            return targetSpeed / ChangeDirectionAcceleration ;
        }
    }
}