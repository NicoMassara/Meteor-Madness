using System;
using _Main.Scripts.Gameplay.Projectile.Components;
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
            var slotData = _projectileSlotSelector.GetNextSlotData();
            
            _meteorAmountToSpawn--;

            var isLastMeteor = _meteorAmountToSpawn == 0;
            if (isLastMeteor)
            {
                _isSpawningBatch = false;
                NotifyAll(ProjectileSpawnerObserverMessage.BatchSpawned);
            }
            
            NotifyAll(ProjectileSpawnerObserverMessage.SpawnMeteor,slotData.IsAbility, slotData.Slot,slotData.DistanceRatio, isLastMeteor);
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

        public void UpdateLevel(int currentLevel) => _projectileSlotSelector.SetLevel(currentLevel);

        public void ClearProjectiles()
        {
            _activeMeteorCount = 0;
            _isSpawningBatch = false;
            _projectileSlotSelector.Restart();
            NotifyAll(ProjectileSpawnerObserverMessage.Clear);
        }
    }
}