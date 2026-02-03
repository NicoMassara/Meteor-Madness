using System;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerMotor : ObservableComponent
    {
        private readonly ProjectileSlotSelector _projectileSlotSelector;
        private int _activeMeteorCount;
        private int _meteorAmountToSpawn;
        private bool _isSpawningBatch;

        public ProjectileSpawnerMotor(IProjectileSpawnData spawnData)
        {
            _projectileSlotSelector = new ProjectileSlotSelector(spawnData);
        }

        public void DoStartMeteorBatch()
        {
            _meteorAmountToSpawn = _projectileSlotSelector.CreateBatchData();
            _isSpawningBatch = true;
        }

        private void SpawnMeteor()
        {
            var isAbility = false;
            
            var slotData = _projectileSlotSelector.GetNextSlotData();
            
            _meteorAmountToSpawn--;

            var isLastMeteor = _meteorAmountToSpawn == 0;
            if (isLastMeteor)
            {
                _isSpawningBatch = false;
                NotifyAll(ProjectileSpawnerObserverMessage.BatchSpawned);
            }
            
            
            NotifyAll(ProjectileSpawnerObserverMessage.SpawnMeteor,isAbility, slotData.Slot,slotData.DistanceRatio, isLastMeteor);
        }

        public void DoSpawnMeteorRing()
        {
            NotifyAll(ProjectileSpawnerObserverMessage.SpawnRing);
        }

        public void SpawnNextMeteorFromBatch()
        {
            if (_isSpawningBatch)
            {
                SpawnMeteor();
            }
        }

        public void AddActiveMeteor() => _activeMeteorCount++;
        public void RemoveActiveMeteor() => _activeMeteorCount--;

        public void ClearProjectiles()
        {
            _activeMeteorCount = 0;
            _isSpawningBatch = false;
            NotifyAll(ProjectileSpawnerObserverMessage.Clear);
        }
    }

    internal class ProjectileSlotSelector
    {
        private const int SlotsAmount = GameParameters.GameplayValues.SpawnLevelAmount;
        private readonly IProjectileSpawnData _data;
        private SlotData[] _currentBatch;
        private int _currentBatchIndex;
        private int _currentLevel;
        
        public ProjectileSlotSelector(IProjectileSpawnData data)
        {
            _data = data;
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;
        }

        public int CreateBatchData()
        {
            var spawnData = _data.GetDataByIndex(_currentLevel);

            var selectedSlot = Random.Range(0, SlotsAmount);
            var spawnType = spawnData.SpawnTypeRange.RandomRange;
            var selectedAmount = spawnData.AmountRange.RandomRange;
            var innerBatchDistance = spawnData.InnerBatchDistanceRange.RandomRange;

            _currentBatch = new SlotData[selectedAmount];

            for (var i = selectedAmount - 1; i >= 0; i--)
            {
                if (i != selectedAmount - 1)
                {
                    var offset = spawnData.SlotRange.RandomRange;

                    switch (spawnType)
                    {
                        case SpawnType.Random:
                            offset *= Random.value > 0.5f ? -1 : 1;
                            selectedSlot += offset;
                            break;

                        case SpawnType.Ascendent:
                            selectedSlot += offset;
                            break;

                        case SpawnType.Descendent:
                            selectedSlot -= offset;
                            break;
                    }

                    selectedSlot = (int)Mathf.Repeat(selectedSlot, SlotsAmount);
                }

                _currentBatch[i] = new SlotData
                {
                    Slot = selectedSlot,
                    DistanceRatio = (i == 0) ? 
                        spawnData.NextBatchDistanceRange.RandomRange : 
                        innerBatchDistance
                };
            }

            _currentBatchIndex = selectedAmount - 1;
            return selectedAmount;
        }
        
        public SlotData GetNextSlotData()
        {
            var value = _currentBatch[_currentBatchIndex];
            _currentBatchIndex--;
            
            return value;
        }
    }
}