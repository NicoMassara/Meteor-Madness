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
        private BatchType _currentBatchType;
        private int _amountToSpawn;
        private bool _isSpawningSpecialBatch;
        private bool _isLastSpecialBatch;
        private int _currentLevel;
        public bool IsSpawningBatch { get; private set; }
        
        public event Action OnBatchSpawned;
        public event Action OnBatchCreated;

        public event Action<BatchType> OnSpecialBatchStarted;
        public event Action<BatchType> OnSpecialBatchFinished;
        

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

            ringBatch.OnLastBatchedCreated += () =>
            {
                _isLastSpecialBatch = true;
            };

            // === SlowMo == //
            var slowMoBatch = new SpecialBatchController(data.slowMotionData.data, data.slowMotionData.weights,
                projectileValue);
            
            ringBatch.OnLastBatchedCreated += () =>
            {
                _isLastSpecialBatch = true;
            };
            
            // === Automatic == //
            var automaticBatch = new SpecialBatchController(data.automaticData.data, data.automaticData.weights,
                projectileValue);
            
            ringBatch.OnLastBatchedCreated += () =>
            {
                _isLastSpecialBatch = true;
            };
            
            _batchTypeDic = new Dictionary<BatchType, IBatchTypeCreator>
            {
                {BatchType.Default, defaultBatch},
                {BatchType.Ring, ringBatch},
                {BatchType.SlowedDown, slowMoBatch},
                {BatchType.Automatic, automaticBatch},
            };
            
            _currentBatchType = BatchType.Default;
        }

        public void RestartData()
        {
            _spawnTypeSelector.RestartData();
            _batchCreator.RestartValues();
            _currentBatchType = BatchType.Default;

            foreach (var item in _batchTypeDic.Values)
            {
                item.RestartValues();
            }
        }
        
        public void ChangeBatchType(BatchType newType)
        {
            if(newType == _currentBatchType) return;
            
            _currentBatchType = newType;
            _spawnTypeSelector.ClearHistory();
        }

        public int CreateBatch()
        {
            if (_currentBatchType is BatchType.Ring or  BatchType.SlowedDown or BatchType.Automatic
                && _isSpawningSpecialBatch == false)
            {
                _isSpawningSpecialBatch = true;
                _isLastSpecialBatch = false;
                OnSpecialBatchStarted?.Invoke(_currentBatchType);
            }

            var tempData = GetBatchData();
            _amountToSpawn = _batchCreator.CreateBatchData(tempData, GetProjectileValue);
            IsSpawningBatch = true;
            OnBatchCreated?.Invoke();
            
            return _amountToSpawn;
        }

        public SlotData GetSlotDataFromBatch()
        {
            var data = GetSlotData();
            _amountToSpawn--;
            
            if (_amountToSpawn == 0)
            {
                if (_isLastSpecialBatch)
                {
                    _isSpawningSpecialBatch = false;
                    RestartByType(_currentBatchType);
                    OnSpecialBatchFinished?.Invoke(_currentBatchType);
                }
                
                IsSpawningBatch = false;
                OnBatchSpawned?.Invoke();
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

        private void RestartByType(BatchType batchType) => _batchTypeDic[batchType].RestartValues();

        private BatchSpawnData GetBatchData() 
            => _batchTypeDic[_currentBatchType].GetBatchSpawnData(GetSpawnType);
        private float GetProjectileValue(int index, int maxAmount) 
            => _batchTypeDic[_currentBatchType].GetProjectileValue(index, maxAmount);
        private SlotData GetSlotData() 
            => _batchCreator.GetNextSlotData();
        private SpawnType GetSpawnType(Dictionary<SpawnType, int> currentWeights, ISpawnBaseWeights baseWeights) 
            => _spawnTypeSelector.GetRandomItem(currentWeights, baseWeights);
    }
}