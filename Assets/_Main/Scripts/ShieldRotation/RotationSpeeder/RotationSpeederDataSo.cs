using UnityEngine;

namespace _Main.Scripts.ShieldRotation.RotationSpeeder
{
    public interface IRotationSpeederData
    {
        public float MaxSpeed { get; }
        public float AccelerationVel { get; }
        public float DeAccelerationVel { get; }
    }

    [CreateAssetMenu(fileName = "So_ShieldRotation_Speeder_Default", menuName = "Shield Rotation/Speeder Data", order = 0)]
    public class RotationSpeederDataSo : ScriptableObject, IRotationSpeederData
    {
        [Min(1)]
        [SerializeField] private float maxSpeed;
        [Min(1)]
        [SerializeField] private float accelerationVel;
        [Min(1)]
        [SerializeField] private float deAccelerationVel;

        public float MaxSpeed => maxSpeed;
        public float AccelerationVel => accelerationVel;
        public float DeAccelerationVel => deAccelerationVel;
    }
}