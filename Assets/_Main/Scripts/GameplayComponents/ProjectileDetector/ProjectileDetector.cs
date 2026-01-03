using System;
using UnityEngine;

namespace _Main.Scripts.GameplayComponents.ProjectileDetector
{
    public class ProjectileDetector
    {
        private readonly ProjectileDetectorData _data;
        private readonly Collider2D[] _colliders = new Collider2D[10];
        private readonly IMovement _movement;
        private readonly int _angleSlots;
        private const float CheckInterval = 0.01f;
        private ITargetable _activeTarget;
        private int _targetSlot = -1;
        private float _lastCheckTime;
        public bool HasActiveTarget { get; private set; }

        public event Action OnTargetFound;
        public event Action OnTargetLost;
        
        public ProjectileDetector(ProjectileDetectorData data, IMovement movement, int angleSlots)
        {
            _data = data;
            _movement = movement;
            _angleSlots = angleSlots;
        }
        
        public void CheckForProjectile()
        {
            if(HasActiveTarget) return;
            var shieldPosition = _movement.Position;
            var tempTarget = GetNearestTarget(shieldPosition);
            
            if(tempTarget == null) return;
            
            var targetPos = tempTarget.Position;
            var direction = (targetPos - shieldPosition).normalized;
            
            Debug.DrawRay(_movement.Position,
                direction * Mathf.Infinity, Color.green, _data.CheckRadius);
            
            _targetSlot = GetSlotFromPosition(targetPos);
            _activeTarget = tempTarget;
            _activeTarget.DisableTargetable();
            _activeTarget.OnDeath += Target_OnDeathHandler;
            HasActiveTarget = true;
            
            OnTargetFound?.Invoke();
        }

        public int GetNearestProjectileSlot()
        {
            var shieldPosition = _movement.Position;
            var tempTarget = GetNearestTarget(shieldPosition);
            if(tempTarget == null) return -1;
            var targetPos = tempTarget.Position;
            
            return GetSlotFromPosition(targetPos);
        }

        public int GetSlotDiff()
        {
            return Mathf.Abs(_targetSlot - _movement.GetCurrentSlot());
        }
        
        public int GetSlotDirection()
        {
            if (_activeTarget == null) return 0;

            int rawDiff = _targetSlot - _movement.GetCurrentSlot();
            
            int half = _angleSlots / 2;
            int diff = ((rawDiff % _angleSlots) + _angleSlots) % _angleSlots; 
            if (diff > half) diff -= _angleSlots;
            
            const float tolerance = 0.05f;
            
            return Mathf.Abs(diff) < tolerance ? 0 : (int)Mathf.Sign(diff);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private ITargetable GetNearestTarget(Vector2 position, float checkRadius = Mathf.Infinity)
        {
            if((Time.time - _lastCheckTime) < CheckInterval) return null;
            
            var filter = new ContactFilter2D
            {
                layerMask = _data.ProjectileLayer,
                useLayerMask = true
            };

            var hitCount = Physics2D.OverlapCircle(position, checkRadius, filter,_colliders);
            
            _lastCheckTime = Time.time;
            return hitCount == 0 ? 
                null : 
                _colliders[GetNearestProjectile(position, hitCount)].GetComponent<ITargetable>();
        }
        
        private int GetSlotFromPosition(Vector2 position)
        {
            Vector2 dir = position - Vector2.zero;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // ángulo en grados
            angle = Mathf.Repeat(angle, 360f);
            
            int slot = Mathf.FloorToInt(angle / (360f / _angleSlots)) % _angleSlots;
            return slot;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private int GetNearestProjectile(Vector2 shieldPosition, int hitCount)
        {
            var minDistance = float.MaxValue;
            var selectedIndex = -1;
            
            for (int i = 0; i < hitCount; i++)
            {
                var item = _colliders[i].GetComponent<ITargetable>();
                if (item == null) continue;
                if (item.CanBeTargeted == false) continue;
                var distance = Vector2.Distance(item.Position, shieldPosition);

                if (distance < minDistance)
                {
                    selectedIndex = i;
                    minDistance = distance;
                }
            }

            return selectedIndex;
        }
        
        private void Target_OnDeathHandler()
        {
            _activeTarget.OnDeath -= Target_OnDeathHandler;
            _activeTarget = null;
            _targetSlot = -1;
            HasActiveTarget = false;
            
            OnTargetLost?.Invoke();
        }
    }

    [Serializable]
    public class ProjectileDetectorData
    {
        public LayerMask ProjectileLayer;
        [Range(10,30)]
        public int CheckRadius;
    }
}