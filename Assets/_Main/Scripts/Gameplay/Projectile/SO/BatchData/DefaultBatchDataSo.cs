using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.SO
{
    [CreateAssetMenu(fileName = "So_BatchData_Default_Default", menuName = "Scriptable Objects/Projectile Spawn/Batch Data/Default", order = 0)]
    internal class DefaultBatchDataSo : ScriptableObject, IDefaultBatchData
    {
        [Min(1)]
        [SerializeField] private float projectileSpeed;
        [SerializeField] private SpawnData[] fixedBatchValues;
        [SerializeField] private SpawnData randomBatchValues;

        public float ProjectileSpeed => projectileSpeed;
        public BatchType BatchType => BatchType.Default;
        public SpawnData[] FixedBatchValues => fixedBatchValues;
        public SpawnData RandomBatchValues => randomBatchValues;

        private void OnValidate()
        {
            SetArray();
        }
        
        private void OnEnable()
        {
            SetArray();
        }

        private void SetArray()
        {
            if (fixedBatchValues == null || fixedBatchValues.Length != GameParameters.GameplayValues.SpawnLevelAmount)
            {
                fixedBatchValues = new SpawnData[GameParameters.GameplayValues.SpawnLevelAmount];
            }
        }
    }
}