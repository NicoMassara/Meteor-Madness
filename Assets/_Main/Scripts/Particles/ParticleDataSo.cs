using UnityEngine;

namespace _Main.Scripts.Particles
{
    [CreateAssetMenu(fileName = "SO_ParticleData_Name", menuName = "Scriptable Objects/Particles/Particle Data", order = 0)]
    public class ParticleDataSo : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private float rotationOffset;
        [Range(0.1f, 15f)] 
        [SerializeField] private float timeToFade;
        [Range(0.1f, 10)]
        [SerializeField] private float fadeSpeed;
        [Range(0f, 5)] 
        [SerializeField] private float fadeScale;

        public Sprite Sprite => sprite;
        public Vector3 PositionOffset => positionOffset;
        public float RotationOffset => rotationOffset;
        public float TimeToFade => timeToFade;
        public float FadeSpeed => fadeSpeed;

        public float FadeScale => fadeScale;
    }
}