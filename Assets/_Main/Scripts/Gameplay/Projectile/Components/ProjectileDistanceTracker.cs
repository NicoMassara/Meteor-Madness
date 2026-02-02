using System;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    public class ProjectileDistanceTracker
    {
        private readonly Transform _cog;
        private readonly float _cofOffset;
        private IProjectile _currentProjectile;
        private Vector2 _startPosition;
        private float _totalDistance;
        private float _targetRatio;
        private bool _hasProjectile;
        

        public event Action OnTargetDistanceReached;

        public ProjectileDistanceTracker(Transform cog, float cofOffset)
        {
            _cog = cog;
            _cofOffset = cofOffset;
        }

        public void Execute()
        {
            if(_hasProjectile == false) return;
            
            if (GetDistanceRatio() >= _targetRatio)
            {
#if UNITY_EDITOR

                if (_currentProjectile is IDebugProjectile debug)
                {
                    debug.TargetRatio = -1;
                }
#endif
                _hasProjectile = false;
                _currentProjectile = null;
                OnTargetDistanceReached?.Invoke();
            }
        }
        

        public void SetProjectile(IProjectile projectile, float targetRatio)
        {
            _currentProjectile = projectile;
            _startPosition = _currentProjectile.Position;
            _targetRatio = Mathf.Clamp01(targetRatio);

#if UNITY_EDITOR

            if (projectile is IDebugProjectile debug)
            {
                debug.TargetRatio =  targetRatio;
            }
#endif
            
            CalculateTotalDistance();
            _hasProjectile = true;
        }

        private void CalculateTotalDistance()
        {
            var centerPos = (Vector2)_cog.position;
            var dir = (_startPosition - centerPos).normalized;
            var targetPos = centerPos + dir * _cofOffset;
            
            _totalDistance =  Vector2.Distance(_startPosition, targetPos);
        }

        private float GetDistanceRatio()
        {
            if (!_hasProjectile || _totalDistance <= Mathf.Epsilon)
                return 0f;

            float traveledDistance = Vector2.Distance(_startPosition, _currentProjectile.Position);
            return traveledDistance / _totalDistance;
        }

        public float GetTargetRadius()
        {
            if (_hasProjectile == false) return 0;
            
            return _cofOffset + (_totalDistance * (1f-_targetRatio));
        }
    }
}