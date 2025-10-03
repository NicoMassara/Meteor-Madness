using System;
using _Main.Scripts.Gameplay.Shield;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.AutoTarget
{
    public class MeteorDetector : ManagedBehavior
    {
        [Header("Meteor Detector")]
        [SerializeField] private LayerMask meteorLayer;
        [Range(2.5f, 15f)] 
        [SerializeField] private float checkDistance = 10f;
        [SerializeField] private Vector2Int slotRange;
        
        private ShieldMovement _movement;
        private readonly Collider2D[] _colliders = new Collider2D[5];
        private ITargetable _activeTarget;
        private int _targetSlot = -1;
        private float _lastCheckTime;
        private const float CheckInterval = 0.01f;
        public bool HasActiveTarget { get; private set; }

        public UnityAction OnTargetLost;
        public UnityAction OnTargetFound;

        public void SetMovement(ShieldMovement movement)
        {
            _movement = movement;
        }

        private ITargetable GetActiveTarget(Vector2 position, float checkRadius = Mathf.Infinity)
        {
            if((Time.time - _lastCheckTime) < CheckInterval) return null;
            
            var hitCount = Physics2D.OverlapCircleNonAlloc(position, checkRadius, _colliders,meteorLayer);
            _lastCheckTime = Time.time;

            return hitCount == 0 ? 
                null : 
                _colliders[GetNearestMeteor(position, hitCount)].GetComponent<ITargetable>();
        }

        public void CheckForMeteor(Vector2 position)
        {
            if(HasActiveTarget) return;
            
            var tempTarget = GetActiveTarget(position);
            
            if(tempTarget == null) return;

            var targetPos = tempTarget.Position;
            var direction = (targetPos - position).normalized;
            
            Debug.DrawRay(position,
                direction * Mathf.Infinity, Color.green, 1f);
            
            _targetSlot = GetSlotFromPosition(targetPos);
            _activeTarget = tempTarget;
            _activeTarget.OnDeath += Target_OnDeathHandler;
            HasActiveTarget = true;
        }

        public void CheckForNearMeteorInSlotRange(Vector2 position, int shieldSlot)
        {
            if(HasActiveTarget) return;
            
            var tempTarget = GetActiveTarget(position, checkDistance);
            
            if(tempTarget == null) return;

            var targetPos = tempTarget.Position;
            var direction = (targetPos - position).normalized;
            var tempSlot = GetSlotFromPosition(targetPos);
            var slotDistance = Mathf.Abs(tempSlot - shieldSlot);
            
            var isInRange = IsSlotInRange(tempSlot, shieldSlot,
                slotRange.x, slotRange.y);
            
            if (isInRange == false)
            {
                Debug.DrawRay(position,
                    direction * checkDistance, Color.yellow, 1f);
                return;
            }
            
            Debug.DrawRay(position,
                direction * checkDistance, Color.green, 1f);

            _targetSlot = tempSlot;
            _activeTarget = tempTarget;
            _activeTarget.OnDeath += Target_OnDeathHandler;
            HasActiveTarget = true;
            OnTargetFound?.Invoke();
        }
        
        private static bool IsSlotInRange(int current, int target, int minDist, int maxDist)
        {
            // diferencia bruta
            int diff = Mathf.Abs(target - current);

            // chequeamos si está en el rango
            return diff >= minDist && diff <= maxDist;
        }

        private int GetSlotFromPosition(Vector2 position)
        {
            var totalSlots = GameParameters.GameplayValues.AngleSlots;
            Vector2 dir = position - Vector2.zero;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // ángulo en grados
            angle = Mathf.Repeat(angle, 360f);
            
            int slot = Mathf.FloorToInt(angle / (360f / totalSlots)) % totalSlots;
            return slot;
        }


        public int GetMeteorAngleSlot()
        {
            return _targetSlot;
        }

        private int GetNearestMeteor(Vector2 shieldPosition, int hitCount)
        {
            var minDistance = float.MaxValue;
            var selectedIndex = -1;
            
            for (int i = 0; i < hitCount; i++)
            {
                var meteorPos = _colliders[i].transform.position;
                var distance = Vector2.Distance(meteorPos, shieldPosition);

                if (distance < minDistance)
                {
                    selectedIndex = i;
                    minDistance = distance;
                }
            }

            return selectedIndex;
        }
        
        public int GetSlotDirection(int shieldSlot)
        {
            if (_activeTarget == null) return 0;
            var totalSlots = GameParameters.GameplayValues.AngleSlots;
            
            
            int rawDiff = _targetSlot - shieldSlot;
            
            int half = totalSlots / 2;
            int diff = ((rawDiff % totalSlots) + totalSlots) % totalSlots; 
            if (diff > half) diff -= totalSlots;
            
            const float tolerance = 0.5f;
            
            return Mathf.Abs(diff) < tolerance ? 0 : (int)Mathf.Sign(diff);
        }

        private void Target_OnDeathHandler()
        {
            _activeTarget.OnDeath -= Target_OnDeathHandler;
            _activeTarget = null;
            _targetSlot = -1;
            HasActiveTarget = false;
            
            OnTargetLost?.Invoke();
        }
        
        private Vector2 CheckMeteorPosition()
        {
            return transform.position;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < GameParameters.GameplayValues.AngleSlots; i++)
            {
                float angle = GetAngleBySlot(i);
                Gizmos.DrawLine(CheckMeteorPosition(), GetPositionByAngle(angle, checkDistance));
            }
            
            Gizmos.DrawWireSphere(CheckMeteorPosition(), checkDistance);

            if (_movement == null) return;
            
            Gizmos.color = Color.green;

            for (int i = -slotRange.y; i <= slotRange.y; i++)
            {
                if (Mathf.Abs(i) < slotRange.x)
                    continue;

                int slotIndex = _movement.GetCurrentSlot() + i;
                float angle = GetAngleBySlot(slotIndex);
                Gizmos.DrawLine(CheckMeteorPosition(), GetPositionByAngle(angle, checkDistance));
            }
        }

        private float GetAngleBySlot(int selectedSlot)
        {
            float anglePerSlot = 360f / GameParameters.GameplayValues.AngleSlots;
            int slot = ((selectedSlot % GameParameters.GameplayValues.AngleSlots) 
                        + GameParameters.GameplayValues.AngleSlots) 
                       % GameParameters.GameplayValues.AngleSlots; // módulo positivo
            return slot * anglePerSlot;
        }

        private Vector3 GetPositionByAngle(float angle, float radius)
        {
            float radians = angle * Mathf.Deg2Rad;
            Vector2 point = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * radius;
            return CheckMeteorPosition() + point; // importante: relativo al centro
        }
    }
}