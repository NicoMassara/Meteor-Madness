using System;
using _Main.Scripts.Gameplay.Projecitle.Spawner;
using _Main.Scripts.Projectile;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class SpecialBatchController : ProjectileBatchControllerBase<ISingleBatchData>
    {
        private int _targetBatches;
        private bool _hasStarted;
        
        public event Action OnLastBatchedCreated;
        
        public SpecialBatchController(ISingleBatchData data, ISpawnWeightsData spawnWeights, float projectileBaseValue) 
            : base(data, spawnWeights, projectileBaseValue)
        {
            
        }

        public override BatchSpawnData GetBatchSpawnData(SelectRandomSpawnDelegate selectRandomSpawn)
        {
            if (_hasStarted)
            {
                _targetBatches--;

                if (_targetBatches <= 0)
                {
                    OnLastBatchedCreated?.Invoke();
                }
            }

            var itemData = BatchData.BatchValues;
            var initialSpawnType = selectRandomSpawn.Invoke(itemData.GetWeights(), SpawnWeights.GetWeights());
            
            if (itemData.GetSpawnDataByType(initialSpawnType, out var spawnData) == false)
            {
                Debug.Log("No Spawn Data found of type :{}");
                return default;
            }
            
            var amount = spawnData.ProjectileAmount.GetRandomRange();

            return new BatchSpawnData
            {
                MovementSpeed = itemData.SpeedMultiplier.GetRandomRange() * BatchData.ProjectileSpeed,
                Amount = amount,
                SlotRange = spawnData.SlotRange.GetRandomRange(),
                InnerDistance = spawnData.InnerBatchDistance.GetRandomRange(),
                NextDistance = spawnData.NextBatchDistance.GetRandomRange(),
                NextSlotRange = spawnData.NextBatchSlotRange.GetRandomRange(),
                SpawnType = amount == 1 ? SpawnType.None : initialSpawnType,
                SlotRangeData = spawnData.SlotRange
            };
        }

        public override float GetProjectileValue(int index, int batchAmount) 
            => index % 2 == 0 ? CalculateValuePerMeteor(batchAmount) : 0;

        public override void RestartValues()
        {

        }
        
        private float CalculateValuePerMeteor(int batchAmount)
        {
            var batchValue = ProjectileBaseValue * 5;
            var finalValue = batchValue / batchAmount;
            return finalValue;
        }
    }
}