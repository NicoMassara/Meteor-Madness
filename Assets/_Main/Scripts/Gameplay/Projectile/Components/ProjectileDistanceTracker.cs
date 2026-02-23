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

        // Fixed: Changed to a private Action to prevent subscription stacking/leaks
        private Action<bool> _onTargetDistanceReached;

        public ProjectileDistanceTracker(Transform cog, float cofOffset)
        {
            _cog = cog;
            _cofOffset = cofOffset;
        }

        public void Execute()
        {
            if (!_hasProjectile) return;

            if (GetDistanceRatio() >= _targetRatio)
            {
                //Debug.Log($"Tracker Reached Target, {Time.realtimeSinceStartup}");
                
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
                // Clean up existing projectile before starting a new one to prevent orphaned listeners
                //Debug.LogWarning("Projectile is still active! Cleaning up old reference.");
                RemoveCurrentProjectile();
            }

            // Fixed: Directly assign the callback instead of using +=
            _onTargetDistanceReached = callback;
            _isLastFromBatch = isLast;
            _currentProjectile = projectile;
            _startPosition = _currentProjectile.Position;
            _targetRatio = Mathf.Clamp01(targetRatio);
            _currentProjectile.OnObjectDisabled += OnProjectileDisabledHandler;
            
            CalculateTotalDistance();
            _hasProjectile = true;

            //Debug.Log($"Projectile Added, Ratio: {targetRatio}, Time:{Time.realtimeSinceStartup}");

    #if UNITY_EDITOR
            if (projectile is IDebugProjectile debug)
            {
                debug.TargetRatio = targetRatio;
            }
    #endif

            // Fixed: Immediate check in case the projectile is already at/past the target ratio
            if (GetDistanceRatio() >= _targetRatio)
            {
                RemoveCurrentProjectile();
            }
        }

        public void ClearProjectileSilently()
        {
            if (_currentProjectile != null)
            {
                _currentProjectile.OnObjectDisabled -= OnProjectileDisabledHandler;
            }
            
            _hasProjectile = false;
            _currentProjectile = null;
            _onTargetDistanceReached = null;
        }

        private void RemoveCurrentProjectile()
        {
            if (_currentProjectile == null)
            {
                return;
            }

            //Debug.Log("Projectile Removed At: " + Time.realtimeSinceStartup);
            
            // Unsubscribe to prevent memory leaks
            _currentProjectile.OnObjectDisabled -= OnProjectileDisabledHandler;
            
            // Cache data for the callback
            var callback = _onTargetDistanceReached;
            bool lastFromBatch = _isLastFromBatch;

            // Reset state before invoking to prevent re-entry bugs
            _hasProjectile = false;
            _currentProjectile = null;
            _onTargetDistanceReached = null;

            // Invoke the callback
            callback?.Invoke(lastFromBatch);
        }

        private void CalculateTotalDistance()
        {
            var centerPos = (Vector2)_cog.position;
            var dir = (_startPosition - centerPos).normalized;
            var targetPos = centerPos + dir * _cofOffset;
            
            _totalDistance = Vector2.Distance(_startPosition, targetPos);
        }

        private float GetDistanceRatio()
        {
            // Use a small epsilon to avoid Division by Zero
            if (!_hasProjectile || _totalDistance <= 0.0001f)
                return 0f;

            float traveledDistance = Vector2.Distance(_startPosition, _currentProjectile.Position);
            return traveledDistance / _totalDistance;
        }

        public float GetTargetRadius()
        {
            if (!_hasProjectile) return 0;
            
            return _cofOffset + (_totalDistance * (1f - _targetRatio));
        }
        
        private void OnProjectileDisabledHandler()
        {
            //Debug.Log("Projectile Disabled");
            RemoveCurrentProjectile();
        }
    }
}