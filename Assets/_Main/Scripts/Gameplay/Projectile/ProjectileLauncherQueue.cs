using System.Collections.Generic;
using _Main.Scripts.InspectorTools;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileLauncherQueue : ManagedBehavior, IUpdatable
    {
        [SerializeField] private ProjectileSpawnSettings spawnSettings;
        private readonly ProjectileDistanceTracker _distanceTracker = new ProjectileDistanceTracker();
        private readonly Queue<IProjectile> _projectileQueue = new Queue<IProjectile>();
        private ulong _firstSpawnTimerId;
        private bool _canLaunch = false;
        private bool _gameplayActive;
        [SerializeField] [ReadOnly] private int projectileCount;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        public TickGroup SelfTickGroup { get; } = TickGroup.HalfTick;
        public float LastUpdateTime { get; set; }

        private void Awake()
        {
            SetEventBus();
        }

        public void ExecuteUpdate()
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
            GameEventCaller.Subscribe<AbilitiesEvents.NotifyIsActive>(EventBus_Ability_SetActive);
            //
            GameEventCaller.Subscribe<MeteorEvents.RingActive>(EventBus_Meteor_RingActive);
            //
            GameEventCaller.Subscribe<ProjectileEvents.Add>(EventBus_Projectile_Add);
            GameEventCaller.Subscribe<ProjectileEvents.RequestSpawn>(EventBus_Projectile_SpawnRequest);
            GameEventCaller.Subscribe<ProjectileEvents.ClearQueue>(EnventBus_Projectile_ClearQueue);
            GameEventCaller.Subscribe<ProjectileEvents.DisableSpawn>(EventBus_Projectile_DisableSpawn);
            GameEventCaller.Subscribe<ProjectileEvents.EnableSpawn>(EventBus_Projectile_EnableSpawn);
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
            
            _firstSpawnTimerId = TimerManager.Add(new TimerData
            {
                Time = 1f,
                OnEndAction = () =>
                {
                    _canLaunch = true;
                }
            }, SelfUpdateGroup);
        }
        
        private void EventBus_Projectile_DisableSpawn(ProjectileEvents.DisableSpawn input)
        {
            _gameplayActive = false;
            _canLaunch = false;
            _distanceTracker.ClearValues();
            TimerManager.Remove(ref _firstSpawnTimerId);
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