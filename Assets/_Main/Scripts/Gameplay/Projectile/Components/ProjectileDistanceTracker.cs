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
        private bool _isLastFromBatch;

        public event Action<bool> OnTargetDistanceReached;

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
             
                Debug.Log($"Tracker, {Time.realtimeSinceStartup}");
                
#if UNITY_EDITOR

                if (_currentProjectile is IDebugProjectile debug)
                {
                    debug.TargetRatio = -1;
                }
#endif
                RemoveCurrentProjectile();
            }
        }
        

        public void SetProjectile(IProjectile projectile, float targetRatio, bool isLast, Action<bool> callback)
        {
            if (_currentProjectile != null)
            {
                Debug.Log($"Projectile is still active!");
            }

            OnTargetDistanceReached = callback;
            _isLastFromBatch = isLast;
            _currentProjectile = projectile;
            _startPosition = _currentProjectile.Position;
            _targetRatio = Mathf.Clamp01(targetRatio);
            _currentProjectile.OnObjectDisabled += OnProjectileDisabledHandler;
            
            Debug.Log("Projectile Added At: " + Time.realtimeSinceStartup);

#if UNITY_EDITOR

            if (projectile is IDebugProjectile debug)
            {
                debug.TargetRatio =  targetRatio;
            }
#endif
            
            CalculateTotalDistance();
            _hasProjectile = true;
        }

        public void ClearProjectileSilently()
        {
            _hasProjectile = false;
            _currentProjectile = null;
        }

        private void RemoveCurrentProjectile()
        {
            if (_currentProjectile == null)
            {
                Debug.Log("Projectile could not be Removed");
                return;
            }

            Debug.Log("Projectile Removed At: " + Time.realtimeSinceStartup);
            _currentProjectile.OnObjectDisabled -= OnProjectileDisabledHandler;
            _hasProjectile = false;
            _currentProjectile = null;
            OnTargetDistanceReached?.Invoke(_isLastFromBatch);
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
        
        private void OnProjectileDisabledHandler()
        {
            Debug.Log("Projectile Disabled");
            RemoveCurrentProjectile();
        }
    }
}