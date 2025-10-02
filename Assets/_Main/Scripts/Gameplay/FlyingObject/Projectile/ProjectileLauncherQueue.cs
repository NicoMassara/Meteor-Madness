using System.Collections.Generic;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
namespace _Main.Scripts.Gameplay.FlyingObject.Projectile
{
    public class ProjectileLauncherQueue : ManagedBehavior, IUpdatable
    {
        [SerializeField] private ProjectileSpawnSettings spawnSettings;
        private readonly ProjectileDistanceTracker _distanceTracker = new ProjectileDistanceTracker();
        private readonly Queue<IProjectile> _projectileQueue = new Queue<IProjectile>();
        private ulong _firstSpawnTimerId;
        private bool _canLaunch = false;
        
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
            var spawnPosition = spawnSettings.GetSpawnPosition();
            var direction = spawnSettings.GetCenterOfGravity() - spawnPosition;
            
            GameManager.Instance.EventManager.Publish(
                new ProjectileEvents.Request
            {
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
        }

        private void AddProjectile(IProjectile projectile)
        {
            projectile.EnableMovement = false;
            _projectileQueue.Enqueue(projectile);
        }

        #region Event Bus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<AbilitiesEvents.SetActive>(EventBus_Ability_SetActive);
            eventManager.Subscribe<ProjectileEvents.Add>(EventBus_Projectile_Add);
            eventManager.Subscribe<MeteorEvents.EnableSpawn>(EnventBus_Meteor_EnableSpawn);
            eventManager.Subscribe<MeteorEvents.RingActive>(EventBus_Meteor_RingActive);
            eventManager.Subscribe<GameModeEvents.Disable>(EventBus_GameMode_Disable);
            eventManager.Subscribe<GameModeEvents.Restart>(EventBus_GameMode_Restart);
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
            _distanceTracker.ClearValues();
            _projectileQueue.Clear();
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