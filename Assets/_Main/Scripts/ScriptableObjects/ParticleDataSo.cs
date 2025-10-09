using _Main.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_ParticleData_Name", menuName = "Scriptable Objects/Particles/Particle Data", order = 0)]
    public class ParticleDataSo : ScriptableObject, IParticleData
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private float rotationOffset;
        [Range(-5, 5)] 
        [SerializeField] private float moveSpeed = 0;
        [Header("Scale Values")]
        [Range(0.1f, 15f)] 
        [SerializeField] private float timeToReachScale;
        [Range(0f, 5)] 
        [SerializeField] private float targetScale;
        [Range(0, 2)] 
        [SerializeField] private float startScale = 1;

        [Header("Fade Values")] 
        [Range(0, 1)] 
        [SerializeField] private float ratioTimeToStartFade = 0.75f;
        [Range(0,5)]
        [SerializeField] private float timeToFade;

        public Sprite Sprite => sprite;

        public Vector3 PositionOffset => positionOffset;

        public float RotationOffset => rotationOffset;

        public float MoveSpeed => moveSpeed;

        public float TimeToReachScale => timeToReachScale;

        public Vector3 TargetScale => targetScale * Vector3.one;

        public Vector3 StartScale => startScale * Vector3.one;

        public float RatioTimeToStartFade => ratioTimeToStartFade;

        public float TimeToFade => timeToFade;
    }
}