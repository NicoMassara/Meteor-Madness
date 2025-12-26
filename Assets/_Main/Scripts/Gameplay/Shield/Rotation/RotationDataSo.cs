using UnityEngine;

namespace MeteorMadness.Gameplay.Shield
{
    [CreateAssetMenu(fileName = "SO_RotationData_Name", menuName = "Scriptable Objects/Rotation/Data", order = 0)]
    public class RotationDataSo : ScriptableObject
    {
        [Range(1,1000)]
        [SerializeField] private float maxAngularSpeed = 550;

        [Tooltip("Degrees per second")]
        [Range(1, 5400)] 
        [SerializeField] private float angularAcceleration = 1500f;
        
        [Tooltip("Degrees per second")]
        [Range(1,5400)] 
        [SerializeField] private float angularDeAcceleration = 2500f;

        [Range(1, 100)]
        [SerializeField] private float snapSpeed = 60;

        [Range(1, 50)] 
        [SerializeField] private float directionResponse  = 10;

        public float MaxAngularSpeed => maxAngularSpeed;
        public float AngularAcceleration => angularAcceleration;
        public float AngularDeAcceleration => angularDeAcceleration;
        public float SnapSpeed => snapSpeed;
        public float DirectionResponse => directionResponse;
    }
}