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
        private bool _isSpawningBatch;
        

        public ProjectileSpawnerMotor(BatchTypeData spawnData)
        {
            _batchSelector = new  ProjectileBatchSelector(spawnData);
            
            _batchSelector.OnBatchSpawned += () =>
            {
                _isSpawningBatch = false;
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
            if (_hasChangedLevel)
            {
                Debug.Log("Spawner has increased level, waiting for current batch to despawn before creating a new one");
                return;
            }

            var amount = _batchSelector.CreateBatch();
            _batchTracker.CreateBatchData(amount);
            _isSpawningBatch = true;
        }

        public void SpawnNextProjectileFromBatch()
        {
            if (_isSpawningBatch == false)
            {
                Debug.Log("Cant spawn Batch Yet");
                return;
            }

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
            Debug.Log($"Level increased to {_currentLevel}, waiting for current batch to finish");
        }

        public void ClearProjectiles()
        {
            _batchSelector.RestartData();
            _batchTracker.RestartData();
            _hasChangedLevel = false;
            _currentLevel = 0;
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

            if (_hasChangedLevel)
            {
                _hasChangedLevel = false;
                Debug.Log("Spawning Batch with new Level Values");
                DoStartMeteorBatch();
                SpawnNextProjectileFromBatch();
            }
        }

        public void InitializeSpawner()
        {
            NotifyAll(ProjectileSpawnerObserverMessage.InitializeFactory);
        }
        
        #endregion
    }
}