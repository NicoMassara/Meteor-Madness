using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    [CreateAssetMenu(fileName = "SO_ShieldMovement_Name", menuName = "Scriptable Objects/Shield/Movement Data", order = 0)]
    public class ShieldMovementDataSo : ScriptableObject
    { 
        [Range(5,50)]
        [SerializeField] private float rotationSpeed = 10f;      // Velocidad de interpolación de la rotación
        [Range(0.015f,0.5f)]
        [SerializeField] private float initialDelay = 0.3f;      // Tiempo inicial antes de repetir
        [Range(0.01f,1f)]
        [SerializeField] private float accelerationRate = 0.05f;
        [Range(0.1f, 50f)]
        [SerializeField] private float decelerationRate = 3f;
        [Range(0.015f,0.1f)]
        [SerializeField] private float minDelay = 0.05f; // Reducción del delay por frame
        
        public float RotationSpeed => rotationSpeed;
        public float InitialDelay => initialDelay;
        public float AccelerationRate => accelerationRate;
        public float MinDelay => minDelay;
        public float DecelerationRate => decelerationRate;
    }
}