using System;
using _Main.Scripts.Projectile;
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

        public ProjectileSpawnerMotor()
        {
            _projectileSlotSelector = new ProjectileSlotSelector();
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
        private readonly ISpawnSlotData _data;
        private readonly int _slotsAmount;
        private SlotData[] _currentBatch;
        private int _currentBatchIndex;
        private int _currentLevel;


        private enum SpawnType
        {
            Random,
            Ascendent,
            Descendent,
            //SamePosition
            
            DEFAULT_MAX
        }

        public ProjectileSlotSelector()
        {
            _slotsAmount = 32;
        }

        public ProjectileSlotSelector(ISpawnSlotData data)
        {
            _data = data;
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;
        }

        public int CreateBatchData()
        {
            const int minAmount = 1;
            const int maxAmount = 16; // max EXCLUSIVO
            const float currentDistance = 0.35f;
            const float distanceToNextBatch = 0.90f; 
            // 0 - Spawn Point
            // 1 - Shield

            var spawnType = (SpawnType)Random.Range(0, (int)SpawnType.DEFAULT_MAX);
            var selectedSlot = Random.Range(0, _slotsAmount);
            var selectedAmount = Random.Range(minAmount, maxAmount);

            _currentBatch = new SlotData[selectedAmount];

            const int minSlotRange = 1;
            const int maxSlotRange = 3;

            for (int i = selectedAmount - 1; i >= 0; i--)
            {
                if (i != selectedAmount - 1)
                {
                    var offset = Random.Range(minSlotRange, maxSlotRange);

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

                    selectedSlot = (int)Mathf.Repeat(selectedSlot, _slotsAmount);
                }

                _currentBatch[i] = new SlotData
                {
                    Slot = selectedSlot,
                    DistanceRatio = (i == 0) ? distanceToNextBatch : currentDistance
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