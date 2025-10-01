using System.Collections.Generic;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.FlyingObject.Projectile
{
    public class ProjectileLauncherQueue : ManagedBehavior, IUpdatable
    {
        [SerializeField] private ProjectileSpawnSettings spawnSettings;
        private readonly ProjectileDistanceTracker _distanceTracker = new ProjectileDistanceTracker();
        private readonly Queue<IProjectile> _projectileQueue = new Queue<IProjectile>();

        public UnityAction OnDistanceCheck;
        
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;
        
        public void ManagedUpdate()
        {
            if (_distanceTracker.HasProjectile == false)
            {
                if (_projectileQueue.Count > 0)
                {
                    LaunchProjectile();
                }
            }
            else 
            {
                if (_distanceTracker.GetDistanceRatio() <= spawnSettings.GetMaxTravelDistance())
                {
                    _distanceTracker.ClearValues();
                    OnDistanceCheck?.Invoke();
                }
            }
        }

        private void LaunchProjectile()
        {
            var temp = _projectileQueue.Dequeue();
            temp.EnableMovement = true;
            _distanceTracker.SetProjectile(temp, spawnSettings.GetCenterOfGravity());
        }

        public void AddProjectile(IProjectile projectile)
        {
            projectile.EnableMovement = false;
            _projectileQueue.Enqueue(projectile);
        }
    }
}