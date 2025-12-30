using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    [CreateAssetMenu(fileName = "SO_RotationData_Name", menuName = "Scriptable Objects/Rotation/Data", order = 0)]
    public class RotationDataSo : ScriptableObject
    {
        [Range(1,1000)]
        [SerializeField] private int maxAngularSpeed = 500;

        [Tooltip("Degrees per second")]
        [Range(1, 10080)] 
        [SerializeField] private int angularAcceleration = 2000;
        
        [Tooltip("Degrees per second")]
        [Range(1,10080)] 
        [SerializeField] private int angularDeAcceleration = 5400;

        [Range(1, 100)]
        [SerializeField] private int snapSpeed = 75;

        [Range(1, 50)] 
        [SerializeField] private int directionResponse  = 35;

        public float MaxAngularSpeed => maxAngularSpeed;
        public float AngularAcceleration => angularAcceleration;
        public float AngularDeAcceleration => angularDeAcceleration;
        public float SnapSpeed => snapSpeed;
        public float DirectionResponse => directionResponse;
    }
}