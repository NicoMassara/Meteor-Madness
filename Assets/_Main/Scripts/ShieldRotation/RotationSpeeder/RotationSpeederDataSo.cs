using UnityEngine;

namespace _Main.Scripts.ShieldRotation.RotationSpeeder
{
    public interface IRotationSpeederData
    {
        public float MaxSpeed { get; }
        public int AccelerateTurnsAmount { get; }
        public int DeAccelerateTurnsAmount { get; }
        public float DegreesStep { get; }
    }

    [CreateAssetMenu(fileName = "So_ShieldRotation_Speeder_Default", menuName = "Shield Rotation/Speeder Data", order = 0)]
    public class RotationSpeederDataSo : ScriptableObject, IRotationSpeederData
    {
        [Min(1)]
        [SerializeField] private float maxSpeed;
        [Min(1)]
        [SerializeField] private int accelerateTurnsAmount = 3;
        [Min(1)]
        [SerializeField] private int deAccelerateTurnsAmount = 2;
        [SerializeField] private float degreesStep;

        public float MaxSpeed => maxSpeed;
        public int AccelerateTurnsAmount => accelerateTurnsAmount;
        public int DeAccelerateTurnsAmount => deAccelerateTurnsAmount;
        public float DegreesStep => degreesStep;
    }
}