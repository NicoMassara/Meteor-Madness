using _Main.Scripts.Gameplay.Projectile.Components;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerMotor : ObservableComponent
    {
        private const int MaxLevelToCheckDeflected = 4;
        private readonly ProjectileBatchController _projectileBatchController;
        private readonly ProjectileBatchTracker _batchTracker;
        private readonly ProjectileRingController _ringController;
        private int _meteorAmountToSpawn;
        private bool _isSpawningBatch;
        private int _currentLevel;
        private int _currentBatchDeflected;

        public ProjectileSpawnerMotor(IProjectileSpawnData spawnData, IProjectileRingData projectileRingData)
        {
            var slotsAmount = GameParameters.GameplayValues.AngleSlots;
            _projectileBatchController = new ProjectileBatchController(spawnData,slotsAmount);
            _ringController = new ProjectileRingController(projectileRingData,slotsAmount);
            _batchTracker = new ProjectileBatchTracker();
            _batchTracker.OnBatchDeflected += OnBatchDeflectedHandler;
            _batchTracker.OnBatchFinished += OnBatchFinishedHandler;
        }

        public void DoStartMeteorBatch()
        {
            _meteorAmountToSpawn = _projectileBatchController.CreateBatchData();
            _batchTracker.CreateBatchData(_meteorAmountToSpawn);
            _isSpawningBatch = true;
        }
        

        public void DoCreateRingBatches()
        {
            _meteorAmountToSpawn = _projectileBatchController.CreateBatchData();
            _batchTracker.CreateBatchData(_meteorAmountToSpawn);
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
            }
            
            NotifyAll(ProjectileSpawnerObserverMessage.SpawnMeteor,slotData, isLastMeteor);
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
    }
}