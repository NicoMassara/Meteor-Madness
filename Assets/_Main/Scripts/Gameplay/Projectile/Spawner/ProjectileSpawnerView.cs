using System;
using _Main.Scripts.EventBus;
using _Main.Scripts.Gameplay.Projectile.Components;
using _Main.Scripts.Gameplay.Projectile.SO;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.AbilitySphere;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Meteor;
using MeteorMadness.Gameplay.Abilities.So;
using MeteorMadness.GlobalValues.Tools.Observer;
using MeteorMadness.GlobalValues.Utilities;
using MeteorMadness.Managers.GameConfig;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal class ProjectileSpawnerView : ManagedBehavior, IObserver,
        ProjectileSpawnerView.IProjectileSpawnerView,
        IUpdatable
    {
        internal interface IProjectileSpawnerView
        {
            public event Action OnProjectileSpawned;
            public event Action OnProjectileReachedTargetRatio;
            public event Action OnBatchSpawned;
            public event Action OnRingFinished;
            public event Action OnRingStarted;
        }


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
        
        #region IProjectileSpawnerView
        
        public event Action OnBatchSpawned;
        
        public event Action OnProjectileReachedTargetRatio;
        public event Action OnProjectileSpawned;
        public event Action OnRingFinished;
        public event Action OnRingStarted;
        
        #endregion

        #region IUpdatable

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.EveryFrame;

        #endregion
        

        private void Start()
        {
            _meteorFactory = new MeteorFactory(meteorPrefab, ()=> doesDebug);
            _abilityFactory = new AbilitySphereFactory(abilityPrefab, abilitySelectorData, ()=> doesDebug);
            _distanceTracker = new ProjectileDistanceTracker(centerOfGravity, centerOfGravityOffset);
            
            _distanceTracker.OnTargetDistanceReached += DistanceTracker_OnTargetDistanceReachedHandler;
        }

        public void ExecuteUpdate(float deltaTime)
        {
            _distanceTracker.Execute();
        }
        
        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case ProjectileSpawnerObserverMessage.SpawnMeteor:
                    HandleSpawnMeteor((SlotData)args[0]);
                    break;
                case ProjectileSpawnerObserverMessage.BatchSpawned:
                    HandleBatchSpawned();
                    break;
                case ProjectileSpawnerObserverMessage.Clear:
                    HandleClear();
                    break;
                case ProjectileSpawnerObserverMessage.BatchDeflected:
                    HandleBatchDeflected();
                    break;
                
                // === Ring === //
                case ProjectileSpawnerObserverMessage.RingStarted:
                    HandleRingStarted();
                    break;
                case ProjectileSpawnerObserverMessage.RingFinished:
                    HandleRingFinished();
                    break;
            }
        }
        
        #region Observer Handlers
        
        private void HandleBatchDeflected()
        {
            ProjectileEventCaller.BatchDeflected();
        }
        
        private void HandleClear()
        {
            _meteorFactory.RecycleAll();
            _abilityFactory.RecycleAll();
        }

        private void HandleSpawnMeteor(SlotData slotData)
        {
            var spawnPosition = GetSpawnPosition(slotData.Slot);
            var direction = (Vector2)centerOfGravity.position - spawnPosition;
            
            var projectile = DoSpawnProjectile(slotData.IsAbility, new ProjectileSpawnValues
            {
                Position = spawnPosition,
                Direction = direction,
                MovementSpeed = slotData.MovementSpeed,
                Value = slotData.FinalValue
            });

            if (projectile == null)
            {
                Debug.LogError("Spawn Failed");
                return;
            }

            OnProjectileSpawned?.Invoke();
            
            if (slotData.DistanceRatio > 0)
            {
                _distanceTracker.SetProjectile(projectile,slotData.DistanceRatio);
            }
            else
            {
                Debug.Log("Instant");
                OnProjectileReachedTargetRatio?.Invoke();
            }
        }
        
        private void HandleBatchSpawned()
        {
            OnBatchSpawned?.Invoke();
        }
        
        private void HandleRingFinished()
        {
            OnRingFinished?.Invoke();
            MeteorEventCaller.RingActive(false);
            
            //Ability Setup should listen to RingActive and then start the timer
            AbilitiesEventCaller.RunTimer();
        }

        private void HandleRingStarted()
        {
            MeteorEventCaller.RingActive(true);
            OnRingStarted?.Invoke();
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

            var angle = AngleCalculations.GetAngleBySlot(selectedAngle, slotAmount);
            var position = AngleCalculations.GetPositionByAngle(angle, spawnRadius);

            return position;
        }

        #region Handlers

        private void DistanceTracker_OnTargetDistanceReachedHandler()
        {
            OnProjectileReachedTargetRatio?.Invoke();
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