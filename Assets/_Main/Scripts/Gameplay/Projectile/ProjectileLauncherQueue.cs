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
        [SerializeField] [ReadOnly] private int projectileCount;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Awake()
        {
            SetEventBus();
        }

        public void ManagedUpdate()
        {
            if (_distanceTracker.HasProjectile == false)
            {
                if (_projectileQueue.Count > 0)
                {
                    LaunchProjectile();
                }
                else if (_projectileQueue.Count == 0 && _canLaunch)
                {
                    RequestProjectile();
                }
            }
            else 
            {
                if (_distanceTracker.GetDistanceRatio() <= spawnSettings.GetMaxTravelDistance())
                {
                    _distanceTracker.ClearValues();

                    if (_projectileQueue.Count == 0 && _canLaunch)
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
            temp.EnableMovement = true;
            _distanceTracker.SetProjectile(temp, spawnSettings.GetCenterOfGravity());
            projectileCount = _projectileQueue.Count;
        }

        private void AddProjectile(IProjectile projectile)
        {
            projectile.EnableMovement = false;
            _projectileQueue.Enqueue(projectile);
            projectileCount = _projectileQueue.Count;
        }

        private void ClearProjectiles()
        {
            _projectileQueue.Clear();
            _distanceTracker.ClearValues();
        }

        #region Event Bus

        private void SetEventBus()
        {
            GameEventCaller.Subscribe<AbilitiesEvents.SetActive>(EventBus_Ability_SetActive);
            GameEventCaller.Subscribe<GameModeEvents.Disable>(EventBus_GameMode_Disable);
            GameEventCaller.Subscribe<GameModeEvents.Restart>(EventBus_GameMode_Restart);
            GameEventCaller.Subscribe<GameModeEvents.Start>(EventBus_GameMode_Start);
            GameEventCaller.Subscribe<MeteorEvents.RingActive>(EventBus_Meteor_RingActive);
            GameEventCaller.Subscribe<MeteorEvents.EnableSpawn>(EnventBus_Meteor_EnableSpawn);
            GameEventCaller.Subscribe<ProjectileEvents.Add>(EventBus_Projectile_Add);
            GameEventCaller.Subscribe<ProjectileEvents.RequestSpawn>(EventBus_Projectile_SpawnRequest);
            GameEventCaller.Subscribe<ProjectileEvents.ClearQueue>(EnventBus_Projectile_ClearQueue);
        }

        private void EventBus_GameMode_Start(GameModeEvents.Start input)
        {
            ClearProjectiles();
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

        private void EventBus_Ability_SetActive(AbilitiesEvents.SetActive input)
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

        private void EventBus_Meteor_RingActive(MeteorEvents.RingActive input)
        {
            _canLaunch = !input.IsActive;
        }

        private void EventBus_Projectile_Add(ProjectileEvents.Add input)
        {
            AddProjectile(input.Projectile);
        }

        private void EventBus_GameMode_Restart(GameModeEvents.Restart input)
        {
            ClearProjectiles();
        }
        
        private void EnventBus_Meteor_EnableSpawn(MeteorEvents.EnableSpawn input)
        {
            if (input.CanSpawn)
            {
                _firstSpawnTimerId = TimerManager.Add(new TimerData
                {
                    Time = 1f,
                    OnEndAction = () =>
                    {
                        _canLaunch = true;
                    }
                }, SelfUpdateGroup);
            }
            else
            {
                _canLaunch = false;
            }
        }
        
        private void EventBus_GameMode_Disable(GameModeEvents.Disable input)
        {
            _canLaunch = false;
            _distanceTracker.ClearValues();
            TimerManager.Remove(_firstSpawnTimerId);
        }

        #endregion
    }
}