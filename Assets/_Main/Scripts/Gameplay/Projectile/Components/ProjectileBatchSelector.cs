using System;
using System.Collections.Generic;
using _Main.Scripts.Common.SelectorByWeight;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileBatchSelector
    {
        private readonly ProjectileBatchCreator _batchCreator;
        private readonly RandomSelector<SpawnType, IWeightsData> _spawnTypeSelector;
        private readonly Dictionary<BatchType, IBatchTypeCreator> _batchTypeDic;
        private int _amountToSpawn;
        private int _currentLevel;
        public BatchType CurrentBatchType { get; private set; }
        
        public event Action<BatchType> OnBatchSpawned;
        public event Action<BatchType> OnProjectileCreated;

        public ProjectileBatchSelector(BatchTypeData data)
        {
            var projectileValue = GameParameters.GameplayValues.BaseMeteorValue;
            
            _batchCreator = new ProjectileBatchCreator(GameParameters.GameplayValues.AngleSlots);
            _batchCreator.OnDebugBatchCreated += debugData =>
            {
                debugData.Level = _currentLevel;
                ProjectileDebugEvents.TriggerBatchCreated(debugData);
            };
            
            _spawnTypeSelector = new RandomSelector<SpawnType, IWeightsData>(10);

            // === Default == //
            var defaultBatch = new DefaultBatchController(data.defaultData.data, data.defaultData.weights,
                GameParameters.GameplayValues.SpawnLevelAmount,
                projectileValue);
            
            // === Ring == //
            var ringBatch = new SpecialBatchController(data.ringData.data, data.ringData.weights,
                projectileValue);

            // === SlowMo == //
            var slowMoBatch = new SpecialBatchController(data.slowMotionData.data, data.slowMotionData.weights,
                projectileValue);
            
            // === Automatic == //
            var automaticBatch = new SpecialBatchController(data.automaticData.data, data.automaticData.weights,
                projectileValue);
            
            
            _batchTypeDic = new Dictionary<BatchType, IBatchTypeCreator>
            {
                {BatchType.Default, defaultBatch},
                {BatchType.Ring, ringBatch},
                {BatchType.SlowedDown, slowMoBatch},
                {BatchType.Automatic, automaticBatch},
            };
            
            CurrentBatchType = BatchType.Default;
            
            UpdateLevel(0);
        }

        public void RestartData()
        {
            _spawnTypeSelector.RestartData();
            _batchCreator.RestartValues();
            CurrentBatchType = BatchType.Default;
            UpdateLevel(0);

            foreach (var item in _batchTypeDic.Values)
            {
                item.RestartValues();
            }
        }

        public void ClearProjectiles()
        {
            _batchCreator.RestartValues();
        }
        
        public int CreateBatch(BatchType batchType)
        {
            if (CurrentBatchType != batchType)
            {
                RestartByType(CurrentBatchType);
                _spawnTypeSelector.ClearHistory();
            }

            CurrentBatchType = batchType;
            
            var tempData = GetBatchData();
            _amountToSpawn = _batchCreator.CreateBatchData(tempData, GetProjectileValue);
            
            return _amountToSpawn;
        }

        public SlotData GetSlotDataFromBatch()
        {
            var data = GetSlotData();
            _amountToSpawn--;
            
            
            OnProjectileCreated?.Invoke(CurrentBatchType);
            
            if (_amountToSpawn == 0)
            {
                data.IsLast = true;
                OnBatchSpawned?.Invoke(CurrentBatchType);
            }
            
            return data;
        }

        public void UpdateLevel(int currentLevel)
        {
            var leveledCreator = (ILeveledBatchTypeCreator)_batchTypeDic[BatchType.Default];

            if (leveledCreator != null)
            {
                leveledCreator.SetLevel(currentLevel);
            }

            _currentLevel = currentLevel;
        }

        public void SetEnableAbilitySpawn(bool input)
        {
            var abilitySpawner = (IAbilitySpawnerBatchType)_batchTypeDic[BatchType.Default];
            
            if (abilitySpawner != null)
            {
                abilitySpawner.SetEnableAbility(input);
            }
            
        }

        private void RestartByType(BatchType batchType) => _batchTypeDic[batchType].RestartValues();

        private BatchSpawnData GetBatchData() 
            => _batchTypeDic[CurrentBatchType].GetBatchSpawnData(GetSpawnType);
        private float GetProjectileValue(int index, int maxAmount) 
            => _batchTypeDic[CurrentBatchType].GetProjectileValue(index, maxAmount);
        private SlotData GetSlotData() 
            => _batchCreator.GetNextSlotData();
        private SpawnType GetSpawnType(Dictionary<SpawnType, int> currentWeights, ISpawnBaseWeights baseWeights) 
            => _spawnTypeSelector.GetRandomItem(currentWeights, baseWeights);
    }
}