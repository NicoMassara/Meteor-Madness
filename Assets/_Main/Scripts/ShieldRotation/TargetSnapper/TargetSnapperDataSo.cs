using UnityEngine;

namespace _Main.Scripts.ShieldRotation.TargetSnapper
{
    public interface ITargetSnapperData
    {
        public float StartVelocity { get; }
        public float DeAccelerationVel { get; }
        public float DeAccelerationForce { get; }
    }
    
    [CreateAssetMenu(fileName = "So_ShieldRotation_Snapper_Default", menuName = "Shield Rotation/Snapper Data", order = 0)]
    public class TargetSnapperDataSo : ScriptableObject, ITargetSnapperData
    {
        [SerializeField] private float startVelocity;
        [Min(1)]
        [SerializeField] private float deAccelerationVel;
        [Tooltip("Log curve force, higher is more aggressive")]
        [Min(0)]
        [SerializeField] private float deAccelerationForce;

        public float StartVelocity => startVelocity;
        public float DeAccelerationVel => deAccelerationVel;
        public float DeAccelerationForce => deAccelerationForce;
    }
}