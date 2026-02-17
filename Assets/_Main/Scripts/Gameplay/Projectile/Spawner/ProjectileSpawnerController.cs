using System.Collections.Generic;
using _Main.Scripts.GlobalValues.Tools;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerController : 
        ProjectileSpawnerController.IProjectileSpawnerController,
        ProjectileSpawnerController.IBaseController,
        ProjectileSpawnerController.ISpawnController
    {
        #region Interfaces

        internal interface IProjectileSpawnerController
        {
            public void InitializeSpawner();
            public void EnableSpawn();
            public void DisableSpawn(bool doesClear);
            public void UpdateLevel(int currentLevel);

            public void NotifyBatchSpawned();
            public void NotifyProjectileHasReachedTargetRatio();
            public void NotifyProjectileDeflected();
            public void NotifyProjectileCollision();
            public void ChangeBatchType(BatchType batchType);
        }
        
        private interface IBaseController
        {

        }

        private interface ISpawnController
        {
            public void DoCreateBatch();
            public void DoSpawnProjectile();
            public void TransitionToIdle();
            public void TransitionToSpawn();
        }

        #endregion
        
        #region FSM

        #region Base

        #region States

        private enum BaseStates
        {
            Enable,
            Disable
        }

        private class BaseState : FsmState<IBaseController> { }

        private class EnableState : BaseState
        {
            
        }

        private class DisableStates : BaseState
        {
            
        }


        #endregion

        #region Controller

        private class BaseFsm : GenericSimpleFsm<BaseStates>
        {
            public void Initialize(IBaseController controller)
            {
                var temp = new List<StateData<BaseState>>
                {
                    new (BaseStates.Enable, new EnableState()),
                    new (BaseStates.Disable, new DisableStates()),
                };
                
                foreach (var state in temp)
                {
                    state.State.InitializeState(controller);
                }
                
                InitializeStates(temp);
                
                TransitionToDisable();
            }
            
            public void TransitionToEnable() => Transition(BaseStates.Enable);
            public void TransitionToDisable() => Transition(BaseStates.Disable);
        }

        private class BaseFsmActionGate : GenericFsmActionGate<BaseStates>
        {
            public bool IsDisable { get; private set; }

            public BaseFsmActionGate(GenericSimpleFsm<BaseStates> fsm)
                : base(fsm) { }

            protected override void OnStateChanged(BaseStates newState)
            {
                IsDisable =  newState == BaseStates.Disable;
            }
        }

        #endregion
        
        #endregion

        #region Spawn

        #region States

        private enum SpawnStates
        {
            Idle,
            CrateBatch,
            Spawn
        }

        private class Spawn_BaseState : FsmState<ProjectileSpawnerController.ISpawnController> { }

        private class Spawn_IdleState : Spawn_BaseState { }

        private class Spawn_SpawnState : Spawn_BaseState
        {
            public override void Awake()
            {
                Controller.DoSpawnProjectile();
                Controller.TransitionToIdle();
            }
        }
        private class Spawn_CrateBatchState : Spawn_BaseState
        {
            public override void Awake()
            {
                Controller.DoCreateBatch();
                Controller.TransitionToSpawn();
            }
        }

        #endregion

        #region Controller

        private class SpawnFsm : GenericSimpleFsm<SpawnStates>
        {
            public void Initialize(ISpawnController controller)
            {
                var temp = new List<StateData<Spawn_BaseState>>
                {
                    new (SpawnStates.Idle, new Spawn_IdleState()),
                    new (SpawnStates.CrateBatch, new Spawn_CrateBatchState()),
                    new (SpawnStates.Spawn, new Spawn_SpawnState()),
                };
                
                foreach (var state in temp)
                {
                    state.State.InitializeState(controller);
                }
                
                InitializeStates(temp);
                
                TransitionToIdle();
            }
            
            public void TransitionToIdle() => Transition(SpawnStates.Idle);
            public void TransitionToCrateBatch() => Transition(SpawnStates.CrateBatch);
            public void TransitionToSpawn() => Transition(SpawnStates.Spawn);
        }

        #endregion
        
        #endregion
        #endregion
        
        private readonly ProjectileSpawnerMotor _motor;
        private BaseFsm _baseFsm;
        private BaseFsmActionGate _baseActionGate;
        private SpawnFsm _spawnFsm;

        private bool _hasSpawnedBatch;
        private bool _hasProjectileReachedTarget;

        public ProjectileSpawnerController(ProjectileSpawnerMotor motor)
        {
            _motor = motor;
            
            InitializeFsm();
        }

        private void InitializeFsm()
        {
            _baseFsm = new BaseFsm();
            _baseFsm.Initialize(this);
            _baseActionGate =  new BaseFsmActionGate(_baseFsm);
            
            _spawnFsm = new SpawnFsm();
            _spawnFsm.Initialize(this);
        }

        #region IProjectileSpawnerController

        public void InitializeSpawner()
        {
            _motor.InitializeSpawner();
        }

        public void EnableSpawn()
        {
            _baseFsm.TransitionToEnable();

            if (_hasSpawnedBatch == false)
            {
                _spawnFsm.TransitionToCrateBatch();
            }
        }

        public void DisableSpawn(bool doesClear)
        {
            _baseFsm.TransitionToDisable();

            if (doesClear)
            {
                _hasProjectileReachedTarget = false;
                _hasSpawnedBatch = false;
                _motor.ClearProjectiles();
            }
        }

        public void NotifyBatchSpawned()
        {
            _hasSpawnedBatch = true;
        }

        public void NotifyProjectileHasReachedTargetRatio()
        {
            _hasProjectileReachedTarget = true;
            if(_baseActionGate.IsDisable) return;
            
            if (_hasSpawnedBatch)
            {
                _spawnFsm.TransitionToCrateBatch();
            }
            else
            {
                _spawnFsm.TransitionToSpawn();
            }
        }

        public void UpdateLevel(int currentLevel)
        {
            _motor.UpdateLevel(currentLevel);
        }

        public void NotifyProjectileDeflected()
        {
            _motor.NotifyProjectileDeflected();
        }

        public void NotifyProjectileCollision()
        {
            _motor.NotifyProjectileCollision();
        }

        public void ChangeBatchType(BatchType batchType)
        {
            _motor.ChangeBatchType(batchType);
        }

        #endregion
        
        #region ISpawnController
        public void DoCreateBatch()
        {
            _hasSpawnedBatch = false;
            _motor.DoStartMeteorBatch();
        }

        public void DoSpawnProjectile()
        {
            _hasProjectileReachedTarget = false;
            _motor.SpawnNextProjectileFromBatch();
        }

        public void TransitionToIdle() => _spawnFsm.TransitionToIdle();
        public void TransitionToSpawn() => _spawnFsm.TransitionToSpawn();

        #endregion

    }
}