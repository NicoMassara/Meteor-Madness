using System;
using System.Collections.Generic;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Gameplay.Projecitle.Spawner;
using _Main.Scripts.Projectile;
using UnityEngine;


namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class DefaultBatchController : ProjectileBatchControllerBase<IDefaultBatchData>,
        IBatchTypeCreator, ILeveledBatchTypeCreator, IAbilitySpawnerBatchType
    {
        private class AbilityCountdown
        {
            private int _currentCount;
            private bool _hasAbilityCountdownActive;
            private Vector2Int _abilityBatchRange = new Vector2Int(2, 5);
            
            public bool IsActive => _hasAbilityCountdownActive;
            
            public bool HasReachedTarget => _currentCount <= 0;

            public void CreateCountdown()
            {
                _currentCount = RandomService.Range(_abilityBatchRange.x, _abilityBatchRange.y+1);
                _hasAbilityCountdownActive = true;
            }

            public void DecreaseCountdown()
            {
                _currentCount--;
            }

            public void Restart()
            {
                _currentCount = int.MaxValue;
                _hasAbilityCountdownActive = false;
            }
        }
        
        private readonly int _levelAmount;
        private readonly AbilityCountdown _abilityCountdown;
        private int _currentLevel;
        private bool _canSpawnAbility;

        
        public DefaultBatchController(IDefaultBatchData data, ISpawnWeightsData spawnWeights, int levelAmount, float projectileBaseValue)
            : base(data, spawnWeights, projectileBaseValue)
        {
            _levelAmount = levelAmount;
            _abilityCountdown = new  AbilityCountdown();
            
            InitializeData();
        }

        private void InitializeData()
        {
            _currentLevel = 0;
            _abilityCountdown.Restart();
            _canSpawnAbility = false;
        }

        public override float GetProjectileValue(int index, int batchAmount) => ProjectileBaseValue;

        public override void RestartValues()
        {
            InitializeData();
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;
        }
        
        public void SetEnableAbility(bool input)
        {
            _canSpawnAbility = input;
        }

        public override BatchSpawnData GetBatchSpawnData(SelectRandomSpawnDelegate selectRandomSpawn)
        {
            return _currentLevel < _levelAmount ? 
                CreateBatchSpawnData(_currentLevel, GetIsAbilityBatch(), selectRandomSpawn) : 
                CreateBatchSpawnData(_levelAmount-1, GetIsAbilityBatch(), selectRandomSpawn);
        }

        private bool GetIsAbilityBatch()
        {
            if (_canSpawnAbility == false)
            {
                return false;
            }

            if (_abilityCountdown.IsActive == false)
            {
                _abilityCountdown.CreateCountdown();
            }
            else
            {
                _abilityCountdown.DecreaseCountdown();

                if (_abilityCountdown.HasReachedTarget)
                {
                    _abilityCountdown.Restart();
                    return true;
                }
            }
            
            return false;
        }


        private BatchSpawnData CreateBatchSpawnData(int index, bool hasAbility, 
            SelectRandomSpawnDelegate selectRandomSpawn)
        {
            var itemData = GetSpawnData(index);
            var initialSpawnType = selectRandomSpawn.Invoke(itemData.GetWeights(), SpawnWeights.GetWeights());
            
            if (itemData.GetSpawnDataByType(initialSpawnType, out var spawnData) == false)
            {
                throw new Exception($"No Spawn Data found of type :{initialSpawnType}");
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
                HasAbility = hasAbility,
                SlotRangeData = spawnData.SlotRange,
                InnerDistanceRangeData =  spawnData.InnerBatchDistance,
            };
        }
        
        private SpawnData GetSpawnData(int index)
        {
            return index >= _levelAmount ? BatchData.RandomBatchValues : BatchData.FixedBatchValues[index];
        }
    }
}