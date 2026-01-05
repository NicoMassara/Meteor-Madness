using UnityEngine;

namespace _Main.Scripts.ShieldRotation.RotationSpeeder
{
    public interface IRotationSpeederData
    {
        public float MaxSpeed { get; }
        public float AccelerationVel { get; }
        public float DeAccelerationVel { get; }
    }

    [CreateAssetMenu(fileName = "So_MovementData_Default", menuName = "Movement/Speeder Data", order = 0)]
    public class RotationSpeederDataSo : ScriptableObject
    {
        
    }
}