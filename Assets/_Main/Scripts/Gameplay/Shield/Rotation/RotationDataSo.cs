using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    [CreateAssetMenu(fileName = "SO_RotationData_Name", menuName = "Scriptable Objects/Rotation/Data", order = 0)]
    public class RotationDataSo : ScriptableObject
    {
        [SerializeField] private int maxAngularSpeed = 500;
        [Tooltip("Degrees per second")]
        [SerializeField] private int angularAcceleration = 2500;
        [Tooltip("Degrees per second")]
        [SerializeField] private int angularDeAcceleration = 8000;
        [SerializeField] private int snapSpeed = 75;
        [SerializeField] private int directionResponse  = 35;
        [Header("FSM Data")]
        [SerializeField] private RotationFsmData rotationFsmData;

        public float MaxAngularSpeed => maxAngularSpeed;
        public float AngularAcceleration => angularAcceleration;
        public float AngularDeAcceleration => angularDeAcceleration;
        public float SnapSpeed => snapSpeed;
        public float DirectionResponse => directionResponse;

        public RotationFsmData RotationFsmData => rotationFsmData;
    }
}