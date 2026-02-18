using _Main.Scripts.Gameplay.Projectile.Components;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerMotor : ObservableComponent
    {
        private const int MaxLevelToCheckDeflected = 4;
        private readonly ProjectileBatchSelector _batchSelector;
        private readonly ProjectileBatchTracker _batchTracker;
        private int _meteorAmountToSpawn;
        private bool _isSpawningRing;
        private bool _hasChangedLevel;
        private int _currentLevel;
        

        public ProjectileSpawnerMotor(BatchTypeData spawnData)
        {
            _batchSelector = new  ProjectileBatchSelector(spawnData);
            
            _batchSelector.OnBatchSpawned += () =>
            {
                NotifyAll(ProjectileSpawnerObserverMessage.BatchSpawned);
            };
            _batchSelector.OnSpecialBatchStarted += (value) =>
            {
                NotifyAll(ProjectileSpawnerObserverMessage.SpecialStarted, value);
            };
            _batchSelector.OnSpecialBatchFinished += (value) =>
            {
                NotifyAll(ProjectileSpawnerObserverMessage.SpecialFinished, value);
            };
            
            _batchTracker = new ProjectileBatchTracker();
            _batchTracker.OnBatchDeflected += OnBatchDeflectedHandler;
            _batchTracker.OnBatchFinished += OnBatchFinishedHandler;
        }

        public void DoStartMeteorBatch()
        {
            /*if (_hasChangedLevel)
            {
                Debug.Log("Spawner has increased level, waiting for current batch to despawn before creating a new one");
                return;
            }*/

            var amount = _batchSelector.CreateBatch();
            _batchTracker.CreateBatchData(amount);
        }

        public void SpawnNextProjectileFromBatch()
        {
            var slotData = _batchSelector.GetSlotDataFromBatch();
            
            NotifyAll(ProjectileSpawnerObserverMessage.SpawnMeteor,slotData);
        }

        public void ChangeBatchType(BatchType newBatchType) => _batchSelector.ChangeBatchType(newBatchType);

        public void NotifyProjectileDeflected()
        {
            _batchTracker.CheckForDeflectedProjectile();
        }

        public void NotifyProjectileCollision()
        {
            _batchTracker.CheckForCollisionProjectile();
        }

        public void UpdateLevel(int currentLevel)
        {
            _currentLevel = currentLevel;
            _batchSelector.UpdateLevel(currentLevel);
            _hasChangedLevel = true;
        }

        public void ClearProjectiles()
        {
            _batchSelector.RestartData();
            _batchTracker.RestartData();
            _hasChangedLevel = false;
            NotifyAll(ProjectileSpawnerObserverMessage.Clear);
        }

        private bool DoesCheckForDeflectMeteors()
        {
            return _currentLevel <= MaxLevelToCheckDeflected;
        }

        #region Handlers
        
        private void OnBatchDeflectedHandler()
        {
            if (DoesCheckForDeflectMeteors())
            {
                NotifyAll(ProjectileSpawnerObserverMessage.BatchDeflected);
            }
        }

        private void OnBatchFinishedHandler()
        {
            if (DoesCheckForDeflectMeteors() == false)
            {
                NotifyAll(ProjectileSpawnerObserverMessage.BatchDeflected);
            }
        }

        public void InitializeSpawner()
        {
            NotifyAll(ProjectileSpawnerObserverMessage.InitializeFactory);
        }
        
        #endregion
    }
}