using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.Gameplay.Shield
{
    [CreateAssetMenu(fileName = "SO_ShieldMovement_Name", menuName = "Scriptable Objects/Shield/Movement Data", order = 0)]
    public class ShieldMovementSo : ScriptableObject
    {
        [Range(0.5f, 100)]
        [SerializeField] private float acceleration = 45;
        [Range(0, 20)]
        [SerializeField] private float drag = 5;
        [Range(0, 360)]
        [SerializeField] private float maxAngularVelocity = 180;
        
        public float Acceleration => acceleration * 1000f;
        public float Drag => drag * 10f;
        public float MaxAngularVelocity => maxAngularVelocity * 1000f;
    }
}