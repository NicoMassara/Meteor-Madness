using System;
using _Main.Scripts.Gameplay.Projectile.Components;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerMotor : ObservableComponent, IProjectileSpawnerMotor
    {
        private readonly ProjectileBatchTracker _tracker;
        private readonly ProjectileBatchSelector _batchSelector;
        
        public event Action OnBatchCreated;
        public event Action OnBatchFinished;

        public ProjectileSpawnerMotor(BatchTypeData batchData)
        {
            _tracker = new ProjectileBatchTracker();
            _tracker.OnBatchDeflected += OnBatchDeflectedHandler;
            _tracker.OnBatchFinished += OnBatchFinishedHandler;
            
            _batchSelector = new ProjectileBatchSelector(batchData);
            _batchSelector.OnBatchSpawned += OnBatchSpawnedHandler;
            _batchSelector.OnProjectileCreated += OnProjectileCreatedHandler;
        }

        public void Initialize()
        {
            NotifyAll(ProjectileSpawnerObserverMessage.Initialize);
        }

        #region Enable / Disable

        public void Clear()
        {
            _tracker.ClearData();
            _batchSelector.ClearProjectiles();
            NotifyAll(ProjectileSpawnerObserverMessage.Clear);
        }
        
        public void RestartValues()
        {
            _tracker.RestartData();
            _batchSelector.RestartData();
        }
        
        #endregion

        public void NotifyProjectileDeflected()
        {
            _tracker.NotifyProjectileDeflected();
        }
        
        public void NotifyProjectileDestroyed()
        {
            _tracker.NotifyProjectileDestroyed();
        }
        
        public void CreateBatch(BatchType batchType)
        {
            var batchAmount = _batchSelector.CreateBatch(batchType);

            if (batchAmount > 0)
            {
                _tracker.CreateBatchData(batchAmount, batchType);
                
                OnBatchCreated?.Invoke();
                NotifyAll(ProjectileSpawnerObserverMessage.BatchCreated, batchType);
            }
            else
            {
                throw new Exception("Batch Cannot be Empty");
            }
        }

        public void SpawnProjectile()
        {
            var data = _batchSelector.GetSlotDataFromBatch();
            NotifyAll(ProjectileSpawnerObserverMessage.SpawnProjectile, data);
        }

        public void UpdateLevel(int level)
        {
            _batchSelector.UpdateLevel(level);
        }

        #region Handlers
        
        private void OnBatchSpawnedHandler(BatchType batchType)
        {
            NotifyAll(ProjectileSpawnerObserverMessage.BatchSpawned, batchType);
        }
        
        private void OnProjectileCreatedHandler(BatchType batchType)
        {
            NotifyAll(ProjectileSpawnerObserverMessage.ProjectileSpawned, batchType);
        }
        
        private void OnBatchDeflectedHandler()
        {
            NotifyAll(ProjectileSpawnerObserverMessage.BatchDeflected);
        }
        
        private void OnBatchFinishedHandler(BatchType batchType)
        {
            OnBatchFinished?.Invoke();
            NotifyAll(ProjectileSpawnerObserverMessage.BatchFinished, batchType);
        }

        #endregion
    }
}