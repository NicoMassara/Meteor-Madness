using System;
using _Main.Scripts.EventBus;
using _Main.Scripts.Gameplay.Projectile.Components;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.AbilitySphere;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Meteor;
using MeteorMadness.Gameplay.Abilities.So;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.GlobalValues.Utilities;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerView : ManagedBehavior,
        IProjectileSpawnerView,
        IUpdatable
    {
        [Header("Spawn Data")]
        [SerializeField] private Transform centerOfGravity;
        [SerializeField] private float spawnRadius;
        [SerializeField] private bool doesDebug;
        [Header("Meteor Factory")]
        [SerializeField] private MeteorView meteorPrefab;
        [Header("Ability Factory")]
        [SerializeField] private AbilitySphereView abilityPrefab;
        [SerializeField] private AbilitySelectorDataSo abilitySelectorData;
        [Header("Distance Tracker")]
        [SerializeField] private float centerOfGravityOffset;


        private ProjectileDistanceTracker _distanceTracker;
        private MeteorFactory _meteorFactory;
        private AbilitySphereFactory _abilityFactory;
        private bool _isLast;
        
        
        #region IProjectileSpawnerView
        
        public event Action<bool> OnProjectileReachedTarget;
        public event Action OnBatchCreated;

        #endregion

        #region IUpdatable

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        #endregion
        

        public void ExecuteUpdate(float deltaTime)
        {
            _distanceTracker?.Execute();
        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case ProjectileSpawnerObserverMessage.Initialize:
                    HandleInitialize();
                    break;
                
                case ProjectileSpawnerObserverMessage.Clear:
                    HandleClear();
                    break;
                
                case ProjectileSpawnerObserverMessage.SpawnProjectile:
                    HandleSpawnProjectile((SlotData)args[0]);
                    break;
                
                case ProjectileSpawnerObserverMessage.BatchCreated:
                    HandleBatchCreated((BatchType)args[0]);
                    break;
                
                case ProjectileSpawnerObserverMessage.ProjectileSpawned:
                    HandleProjectileSpawned((BatchType)args[0]);
                    break;
                
                case ProjectileSpawnerObserverMessage.BatchDeflected:
                    HandleBatchDeflected();
                    break;
                
                case ProjectileSpawnerObserverMessage.BatchFinished:
                    HandleBatchFinished((BatchType)args[0]);
                    break;
                
                case ProjectileSpawnerObserverMessage.BatchSpawned:
                    HandleBatchSpawned((BatchType)args[0]);
                    break;
            }
        }

        #region Observer Handlers
        
        private void HandleInitialize()
        {
            _meteorFactory = new MeteorFactory(meteorPrefab, ()=> doesDebug);
            _abilityFactory = new AbilitySphereFactory(abilityPrefab, abilitySelectorData, ()=> doesDebug);
            _distanceTracker = new ProjectileDistanceTracker(centerOfGravity, centerOfGravityOffset);
        }
        
        private void HandleClear()
        {
            _meteorFactory.RecycleAll();
            _abilityFactory.RecycleAll();
            _distanceTracker.ClearProjectileSilently();
        }

        private void HandleSpawnProjectile(SlotData slotData)
        {
            _isLast = slotData.IsLast;
            
            var spawnPosition = GetSpawnPosition(slotData.Slot);
            var direction = (Vector2)centerOfGravity.position - spawnPosition;
            
            var projectile = DoSpawnProjectile(slotData.IsAbility, new ProjectileSpawnValues
            {
                Position = spawnPosition,
                Direction = direction,
                MovementSpeed = slotData.MovementSpeed,
                Value = slotData.FinalValue,
                Slot = slotData.Slot
            });

            if (projectile == null)
            {
                Debug.LogError("Spawn Failed");
                return;
            }
            
            if (slotData.DistanceRatio > 0)
            {
                _distanceTracker.OnTargetDistanceReached += DistanceTracker_OnTargetDistanceReachedHandler;
                _distanceTracker.SetProjectile(projectile,slotData.DistanceRatio);
            }
            else
            {
                OnProjectileReachedTarget?.Invoke(_isLast);
                ProjectileSpawner.Publish.ProjectileReachedTarget(_isLast);
            }
        }
        
        private void HandleBatchCreated(BatchType batchType)
        {
            ProjectileSpawner.Publish.BatchCreated(batchType);
            OnBatchCreated?.Invoke();
        }
        
        private void HandleBatchSpawned(BatchType batchType)
        {
            ProjectileSpawner.Publish.BatchSpawned(batchType);
        }
        
        private void HandleProjectileSpawned(BatchType batchType)
        {
            ProjectileSpawner.Publish.ProjectileSpawned(batchType);
        }
        
        private void HandleBatchDeflected()
        {
            ProjectileSpawner.Publish.BatchDeflected();
        }
        
        private void HandleBatchFinished(BatchType batchType)
        {
            ProjectileSpawner.Publish.BatchFinished(batchType);
        }
        
        #endregion
        
        private IProjectile DoSpawnProjectile(bool isAbility, ProjectileSpawnValues spawnData)
        {
            return isAbility ? 
                (IProjectile)_abilityFactory.SpawnAbility(spawnData) : 
                (IProjectile)_meteorFactory.SpawnMeteor(spawnData);
        }

        private Vector2 GetSpawnPosition(int selectedAngle)
        {
            var slotAmount = GameParameters.GameplayValues.AngleSlots;

            // I don't know why needs a 180 offset when the shield does not needed it, and I don't want to know it. 
            var angle = AngleCalculations.GetAngleFromSlot(selectedAngle, slotAmount, 180f);
            var position = AngleCalculations.GetPositionByAngle(angle, spawnRadius);

            return position;
        }

        #region Handlers

        private void DistanceTracker_OnTargetDistanceReachedHandler()
        {
            _distanceTracker.OnTargetDistanceReached -= DistanceTracker_OnTargetDistanceReachedHandler;
            
            OnProjectileReachedTarget?.Invoke(_isLast);
            ProjectileSpawner.Publish.ProjectileReachedTarget(_isLast);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (centerOfGravity != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius);
                Gizmos.DrawWireSphere(centerOfGravity.position, centerOfGravityOffset);
                
                if (_distanceTracker != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(centerOfGravity.position, _distanceTracker.GetTargetRadius());
                }
            }
        }
    }
}