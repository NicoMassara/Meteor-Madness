using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileDistanceTracker
    {
        private IProjectile _projectile;
        private Vector2 _targetPosition;
        private float _totalDistance;
        
        public bool HasProjectile => _projectile != null;

        public void SetProjectile(IProjectile projectile, Vector2 targetPosition)
        {
            _projectile = projectile;
            _targetPosition = targetPosition;
            _totalDistance = Vector2.Distance(_projectile.Position, targetPosition);
        }

        public void ClearValues()
        {
            _projectile = null;
            _targetPosition = Vector2.zero;
            _totalDistance = float.PositiveInfinity;
        }

        public float GetDistanceRatio()
        {
            var currentDistance = Vector2.Distance(_projectile.Position, _targetPosition);
            
            return currentDistance/_totalDistance;
        }
    }
}