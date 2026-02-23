using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.SO
{
    [CreateAssetMenu(fileName = "So_BatchData_Single_Default", menuName = "Scriptable Objects/Projectile Spawn/Batch Data/Single", order = 0)]
    internal class SingleBatchDataSo : ScriptableObject, ISingleBatchData
    {
        [SerializeField] private BatchType batchType;
        [Min(1)]
        [SerializeField] private float projectileSpeed;
        [SerializeField] private SpawnData batchValues;

        public SpawnData BatchValues => batchValues;
        public BatchType BatchType => batchType;
        public float ProjectileSpeed => projectileSpeed;
    }
}