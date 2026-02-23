using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerController
    {
        private readonly IProjectileSpawnerMotor _motor;
        private BatchType _pendingBatch;
        private bool _hasActiveBatch;
        private bool _hasPendingBatch;
        private bool _hasLeveledUp;
        private bool _isEnabled;
        
        public ProjectileSpawnerController(IProjectileSpawnerMotor motor)
        {
            _motor = motor;
            _motor.OnBatchCreated += OnBatchCreatedHandler;
        }

        public void Initialize()
        {
            _motor.Initialize();
        }

        public void RestartValues()
        {
            _hasActiveBatch = false;
            _hasPendingBatch = false;
            _hasLeveledUp = false;
            _motor.RestartValues();
        }

        public void Clear()
        {
            _hasActiveBatch = false;
            _hasPendingBatch = false;
            _hasLeveledUp = false;
            _motor.Clear();
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public void CreateBatch(BatchType batchType)
        {
            if(_isEnabled == false) return;
            
            if (_hasActiveBatch == false)
            {
                _hasActiveBatch = true;
                _motor.CreateBatch(batchType);
            }
            else if (_hasLeveledUp && !_hasPendingBatch)
            {
                Debug.Log("Pending Batch added! Spawning when every projectile is destroyed");
                _pendingBatch = batchType;
                _hasPendingBatch = true;
            }
        }

        public void NotifyProjectileReachedTarget()
        {
            TrySpawnProjectile();
        }

        public void NotifyLastProjectileReachedTarget()
        {
            _hasActiveBatch = false;
        }
        
        public void NotifyProjectileDestroyed()
        {
            _motor.NotifyProjectileDestroyed();
        }

        public void NotifyProjectileDeflected()
        {
            _motor.NotifyProjectileDeflected();
        }

        public void UpdateLevel(int currentLevel)
        {
            _hasLeveledUp = true;
            _motor.UpdateLevel(currentLevel);
        }
        
        private void NotifyBatchFinished()
        {
            _motor.OnBatchFinished -= NotifyBatchFinished;

            if (_hasPendingBatch == false) return;
            
            Debug.Log("Level Increased to Batch!");
            _hasPendingBatch = false;
            _hasLeveledUp = false;
            CreateBatch(_pendingBatch);
        }

        private void TrySpawnProjectile()
        {
            if(!_hasActiveBatch) return;
            
            //Debug.Log("Spawning Next Projectile!");
            
            _motor.SpawnProjectile();
        }
        
        private void OnBatchCreatedHandler()
        { 
            _motor.OnBatchFinished += NotifyBatchFinished;
            TrySpawnProjectile();
        }
    }
}