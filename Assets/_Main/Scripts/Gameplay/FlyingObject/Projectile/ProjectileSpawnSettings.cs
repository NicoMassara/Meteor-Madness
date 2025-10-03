using System;
using UnityEngine;

namespace _Main.Scripts.Gameplay.FlyingObject.Projectile
{
    [RequireComponent(typeof(ProjectileSpawnSpeedController))]
    [RequireComponent(typeof(ProjectileSpawnLocationController))]
    public class ProjectileSpawnSettings : MonoBehaviour
    {
        private ProjectileSpawnSpeedController _speed;
        private ProjectileSpawnLocationController _location;

        private float multiplier = 1;

        private void Awake()
        {
            _speed = GetComponent<ProjectileSpawnSpeedController>();
            _location = GetComponent<ProjectileSpawnLocationController>();
        }

        public float GetSpawnRadius()
        {
           return _location.GetSpawnRadius();
        }

        public Vector2 GetPositionByAngle(float currAngle, float radius)
        {
           return _location.GetPositionByAngle(currAngle, radius);
        }

        public Vector2 GetSpawnPosition()
        {
            return GetPositionByAngle(GetSpawnAngle(), GetSpawnRadius());
        }

        public float GetSpawnAngle()
        {
            return _location.GetSpawnAngle();
        }

        public float GetMaxTravelDistance()
        {
            return _speed.GetMaxTravelDistance() * multiplier;
        }

        public float GetMovementMultiplier()
        {
            return _speed.GetMovementMultiplier();
        }

        public Vector2 GetCenterOfGravity()
        {
            return _location.GetCenterOfGravity();
        }

        public void SetMultiplier(float value = 1)
        {
            value = Mathf.Clamp(value, 0.01f, 10);
            multiplier = value;
        }

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if(_location == null || 
               _speed == null) return;
            
            var cog = GetCenterOfGravity();
            float temp = cog.x + GetSpawnRadius();

            foreach (var t in _speed.GetSpawnData())
            {
                var multiplier = t.TravelDistance;
                Gizmos.color = new Color(multiplier, .5f, multiplier/2f, 1);
                Gizmos.DrawWireSphere(cog, multiplier * temp);
            }
        }

        #endregion
    }
}