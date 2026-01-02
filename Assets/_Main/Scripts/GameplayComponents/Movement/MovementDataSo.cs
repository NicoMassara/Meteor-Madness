using UnityEngine;

namespace _Main.Scripts.GameplayComponents.Movement
{
    public interface IMovementData
    {
        public float MaxSpeed { get; }
        public float Acceleration { get; }
        public float Deceleration { get; }
        public float SnapSpeed { get; }
        public float ChangeDirectionAcceleration { get; }
    }
    
    
    public interface IFsmMovementData
    {
        public float StopThreshold { get; }
        public float MinSpeedRatio { get; }
        public float StopChangeDirectionSpeedRatio { get; }
    }

    [CreateAssetMenu(fileName = "So_MovementData_Default", menuName = "Movement Data", order = 0)]
    public class MovementDataSo : ScriptableObject
    {
        
    }
}