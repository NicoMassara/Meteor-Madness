using UnityEngine;

namespace _Main.Scripts.ShieldRotation.AutomaticMovement
{
    public interface IAutomaticMovementData
    {
        public float MaxVel { get; }
        public float AccelerationVel { get; }
        public float DeAccelerationVel { get; }
    }

    [CreateAssetMenu(fileName = "So_ShieldRotation_Automatic_Default", menuName = "Shield Rotation/Automatic Data", order = 0)]
    public class AutomaticMovementDataSo : ScriptableObject, IAutomaticMovementData
    {
        [Min(1)]
        [SerializeField] private float maxVel;
        [Min(1)]
        [SerializeField] private float accelerationVel;
        [Min(1)]
        [SerializeField] private float deAccelerationVel;

        public float MaxVel => maxVel;
        public float AccelerationVel => accelerationVel;
        public float DeAccelerationVel => deAccelerationVel;
    }
}