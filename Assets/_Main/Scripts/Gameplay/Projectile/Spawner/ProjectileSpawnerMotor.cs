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
        private readonly ProjectileBatchController _projectileBatchController;
        private readonly ProjectileRingController _ringController;
        private readonly ProjectileBatchTracker _batchTracker;
        private int _meteorAmountToSpawn;
        private bool _isSpawningBatch;
        private bool _isSpawningRing;
        private int _currentLevel;
        private int _currentBatchDeflected;
        private bool _isLastRingBatch;

        public ProjectileSpawnerMotor(IProjectileSpawnData spawnData, IProjectileRingData projectileRingData)
        {
            var batchData = new ProjectileBatchBase.ProjectileBatchData
            {
                SlotAmount = GameParameters.GameplayValues.AngleSlots,
                ProjectileValue = GameParameters.GameplayValues.BaseMeteorValue
            };
            _projectileBatchController = new ProjectileBatchController(spawnData,batchData,GameParameters.GameplayValues.SpawnLevelAmount);
            _ringController = new ProjectileRingController(projectileRingData,batchData);
            _batchTracker = new ProjectileBatchTracker();
            _ringController.OnLastBatchedCreated += RingController_OnLastBatchedCreatedHandler;
            _batchTracker.OnBatchDeflected += OnBatchDeflectedHandler;
            _batchTracker.OnBatchFinished += OnBatchFinishedHandler;
        }

        public void DoStartMeteorBatch()
        {
            _meteorAmountToSpawn = _projectileBatchController.CreateBatchData();
            _batchTracker.CreateBatchData(_meteorAmountToSpawn);
            _isSpawningBatch = true;
        }

        public void SpawnNextProjectileFromDefaultBatch()
        {
            if (_isSpawningBatch == false) return;
            
            var slotData = _projectileBatchController.GetNextSlotData();
            _meteorAmountToSpawn--;
            
            var isLastMeteor = _meteorAmountToSpawn == 0;
            if (isLastMeteor)
            {
                _isSpawningBatch = false;
                NotifyAll(ProjectileSpawnerObserverMessage.BatchSpawned);
            }
            
            NotifyAll(ProjectileSpawnerObserverMessage.SpawnMeteor,slotData);
        }
        
        public void DoStartRingBatches()
        {
            _meteorAmountToSpawn = _ringController.CreateBatchData();
            _batchTracker.CreateBatchData(_meteorAmountToSpawn);

            if (_isSpawningRing == false)
            {
                _ringController.Initialize();
                NotifyAll(ProjectileSpawnerObserverMessage.RingStarted);
                _isSpawningRing = true;
            }

            _isSpawningBatch = true;
        }

        public void SpawnNextProjectileFromRingBatch()
        {
            if (_isSpawningBatch == false) return;
                
            var slotData = _ringController.GetNextSlotData();
            
            _meteorAmountToSpawn--;

            var isLastMeteor = _meteorAmountToSpawn == 0;
            if (isLastMeteor)
            {
                _isSpawningBatch = false;
                NotifyAll(ProjectileSpawnerObserverMessage.BatchSpawned);

                if (_isLastRingBatch)
                {
                    _isSpawningRing = false;
                    _isLastRingBatch = false;
                    _ringController.Restart();
                    NotifyAll(ProjectileSpawnerObserverMessage.RingFinished);
                }
            }
            
            NotifyAll(ProjectileSpawnerObserverMessage.SpawnMeteor,slotData, isLastMeteor);
        }
        
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
            _projectileBatchController.SetLevel(_currentLevel);
        }

        public void ClearProjectiles()
        {
            _isSpawningBatch = false;
            _projectileBatchController.Restart();
            _batchTracker.RestartData();
            NotifyAll(ProjectileSpawnerObserverMessage.Clear);
        }
        
        private bool DoesCheckForDeflectMeteors()
        {
            return _currentLevel <= MaxLevelToCheckDeflected;
        }
        
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
        
        private void RingController_OnLastBatchedCreatedHandler()
        {
            _isLastRingBatch = true;
        }

        public void InitializeSpawner()
        {
            NotifyAll(ProjectileSpawnerObserverMessage.InitializeFactory);
        }
    }
}