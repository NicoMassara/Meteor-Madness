using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects.GameConfig
{
    [CreateAssetMenu(fileName = "SO_MeteorRingData_Name", menuName = "Scriptable Objects/Game Config/Projectile/Meteor Ring Data", order = 0)]
    public class MeteorRingDataSo : ScriptableObject, IMeteorRingData
    {
        [Range(2,32)]
        [SerializeField] private int meteorAmount = 8;
        [Range(1,10)]
        [SerializeField] private int ringsAmount = 5;
        [Range(1,10)]
        [SerializeField] private int wavesAmount = 3;
        [Range(0,1)]
        [SerializeField] private float delayBetweenRings = 0.15f;
        [Range(0,1)]
        [SerializeField] private float delayBetweenWaves = 0.5f;

        public int MeteorAmount => meteorAmount;
        public int RingsAmount => ringsAmount;
        public int WavesAmount => wavesAmount;
        public float DelayBetweenRings => delayBetweenRings;
        public float DelayBetweenWaves => delayBetweenWaves;
    }
}