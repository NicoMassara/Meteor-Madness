using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield._Experiment
{
    [CreateAssetMenu(fileName = "SO_ShieldDegreeMovement_Name", menuName = "Scriptable Objects/Shield/Degree Movement Data", order = 0)]
    public class ShieldDegreeMovementDataSo : ScriptableObject
    {
        [Range(4,32)]
        [SerializeField] private int slots = 16;                 // Cantidad de posiciones
        [Range(5,50)]
        [SerializeField] private float rotationSpeed = 10f;      // Velocidad de interpolación de la rotación
        [Range(0.015f,0.5f)]
        [SerializeField] private float initialDelay = 0.3f;      // Tiempo inicial antes de repetir
        [Range(0.01f,1f)]
        [SerializeField] private float accelerationRate = 0.05f;
        [Range(0.1f, 10f)]
        [SerializeField] private float decelerationRate = 3f;
        [Range(0.015f,0.1f)]
        [SerializeField] private float minDelay = 0.05f; // Reducción del delay por frame

        public int Slots => slots;
        public float RotationSpeed => rotationSpeed;
        public float InitialDelay => initialDelay;
        public float AccelerationRate => accelerationRate;
        public float MinDelay => minDelay;
        public float DecelerationRate => decelerationRate;
    }
}