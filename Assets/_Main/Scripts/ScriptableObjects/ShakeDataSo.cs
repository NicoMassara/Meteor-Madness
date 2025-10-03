using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_ShakeData_Name", menuName = "Scriptable Objects/Shaker/Shake Data", order = 0)]
    public class ShakeDataSo : ScriptableObject
    {
        [SerializeField] private float shakeTime;
        [Range(0.1f, 500)] 
        [SerializeField] private float shakeIntensity = 20;
        [Range(0,1f)]
        [SerializeField] private float xShakeMagnitude = 0.075f;
        [Range(0,1f)]
        [SerializeField] private float yShakeMagnitude = 0.075f;

        public float ShakeTime => shakeTime;
        public bool DoesLoop => shakeTime <= 0;
        public float ShakeIntensity => shakeIntensity;
        public float XShakeMagnitude => xShakeMagnitude;
        public float YShakeMagnitude => yShakeMagnitude;
    }
}