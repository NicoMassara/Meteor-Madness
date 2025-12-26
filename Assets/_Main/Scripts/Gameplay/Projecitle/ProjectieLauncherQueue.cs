using System.Collections.Generic;
using MeteorMadness.GlobalValues;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Managers;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using Unity.Collections;
using UnityEngine;
namespace _Main.Scripts.Projectile
{
    public class ProjectileLauncherQueue : ManagedBehavior, IUpdatable
    {
        [SerializeField] private ProjectileSpawnSettings spawnSettings;
        private readonly ProjectileDistanceTracker _distanceTracker = new ProjectileDistanceTracker();
        private readonly Queue<IProjectile> _projectileQueue = new Queue<IProjectile>();
        private TimerManager.GeneratedId _firstSpawnTimerId;
        private bool _canLaunch = false;
        private bool _gameplayActive;
        [SerializeField] [ReadOnly] private int projectileCount;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.HalfTarget;

        private void Awake()
        {
            SetEventBus();
        }

        public void ExecuteUpdate(float deltaTime)
        {
            if (_distanceTracker.HasProjectile == false)
            {
                if (_projectileQueue.Count > 0)
                {
                    LaunchProjectile();
                }
                else if (_projectileQueue.Count == 0 && GetCanLaunch())
                {
                    RequestProjectile();
                }
            }
            else 
            {
                if (_distanceTracker.GetDistanceRatio() <= spawnSettings.GetMaxTravelDistance())
                {
                    _distanceTracker.ClearValues();

                    if (_projectileQueue.Count == 0 && GetCanLaunch())
                    {
                        RequestProjectile();
                    }
                }
            }
        }

        private void RequestProjectile()
        {
            ProjectileEventCaller.RequestSpawn(ProjectileType.Meteor);
        }
        
        private void SpawnProjectile(ProjectileType projectileType)
        {
            var spawnPosition = spawnSettings.GetSpawnPosition();
            var direction = spawnSettings.GetCenterOfGravity() - spawnPosition;
            
            ProjectileEventCaller.Spawn(new ProjectileSpawnData
            {
                ProjectileType = projectileType,
                Position = spawnPosition,
                Direction = direction,
                MovementMultiplier = spawnSettings.GetMovementMultiplier(),
            });
        }

        private void LaunchProjectile()
        {
            var temp = _projectileQueue.Dequeue();
            temp.SetEnableMovement(true);
            _distanceTracker.SetProjectile(temp, spawnSettings.GetCenterOfGravity());
            projectileCount = _projectileQueue.Count;
        }

        private void AddProjectile(IProjectile projectile)
        {
            projectile.SetEnableMovement(false);
            _projectileQueue.Enqueue(projectile);
            projectileCount = _projectileQueue.Count;
        }

        private void ClearProjectiles()
        {
            _projectileQueue.Clear();
            _distanceTracker.ClearValues();
            projectileCount = _projectileQueue.Count;
        }

        private bool GetCanLaunch()
        {
            return _canLaunch && _gameplayActive;
        }

        #region Event Bus

        private void SetEventBus()
        {
            AbilitiesEventSubscriber.NotifyIsActive(EventBus_Ability_SetActive);
            //
            MeteorEventSubscriber.RingActive(EventBus_Meteor_RingActive);
            //
            ProjectileEventSubscriber.Add(EventBus_Projectile_Add);
            ProjectileEventSubscriber.RequestSpawn(EventBus_Projectile_SpawnRequest);
            ProjectileEventSubscriber.ClearQueue(EnventBus_Projectile_ClearQueue);
            ProjectileEventSubscriber.DisableSpawn(EventBus_Projectile_DisableSpawn);
            ProjectileEventSubscriber.EnableSpawn(EventBus_Projectile_EnableSpawn);
            //
            
        }

        
        #region Ability

        private void EventBus_Ability_SetActive(AbilitiesEvents.NotifyIsActive input)
        {
            if (input.AbilityType == AbilityType.SlowMotion)
            {
                if (input.IsActive)
                {
                    spawnSettings.SetMultiplier(1.75f);
                }
                else
                {
                    spawnSettings.SetMultiplier(1);
                }
            }
        }

        #endregion
        
        #region Meteor

        private void EventBus_Meteor_RingActive(MeteorEvents.RingActive input)
        {
            _canLaunch = !input.IsActive;
        }
        #endregion
        
        #region Projectile
        
        private void EventBus_Projectile_EnableSpawn(ProjectileEvents.EnableSpawn input)
        {
            _gameplayActive = true;
            ClearProjectiles();

            _firstSpawnTimerId = TimerManager.Add(new TimerData(1f,() =>
            {
                _canLaunch = true;
            }));
        }
        
        private void EventBus_Projectile_DisableSpawn(ProjectileEvents.DisableSpawn input)
        {
            _gameplayActive = false;
            _canLaunch = false;
            _distanceTracker.ClearValues();
            TimerManager.Remove(_firstSpawnTimerId);
        }

        private void EnventBus_Projectile_ClearQueue(ProjectileEvents.ClearQueue input)
        {
            ClearProjectiles();
        }
        
        private void EventBus_Projectile_SpawnRequest(ProjectileEvents.RequestSpawn input)
        {
            if (input.RequestType == EventRequestType.Granted)
            {
                SpawnProjectile(input.ProjectileType);  
            }
        }
        
        private void EventBus_Projectile_Add(ProjectileEvents.Add input)
        {
            AddProjectile(input.Projectile);
        }

        #endregion

        #endregion
    }
}