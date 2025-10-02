using System;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.AutoTarget
{
    public class MeteorDetector
    {
        private readonly LayerMask _meteorLayer;
        private readonly Collider2D[] _colliders = new Collider2D[5];
        private ITargetable _activeTarget;
        private int _currentAngleSlot;
        public bool HasActiveTarget { get; private set; }

        public UnityAction OnTargetLost;
        public UnityAction OnTargetFound;
        

        public MeteorDetector(LayerMask meteorLayer)
        {
            _meteorLayer = meteorLayer;
        }
        
        public void CheckForNearMeteor(Vector2 position, float checkRadius, bool findTarget = false)
        {
            if(HasActiveTarget) return;
            
            var hitCount = Physics2D.OverlapCircleNonAlloc(position, checkRadius, _colliders,_meteorLayer);

            if (hitCount == 0)
            {
                return;
            }
            
            _activeTarget = _colliders[GetNearestMeteor(position, hitCount)].GetComponent<ITargetable>();
            
            if(_activeTarget == null)
                return;

            _activeTarget.OnDeath += Target_OnDeathHandler;
            
            var meteorPos = _activeTarget.Position;
            var dir = (meteorPos - position).normalized;
            var angle = Mathf.Repeat(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, 360f);
            
            Debug.DrawRay(position,
                dir * checkRadius, Color.magenta, 0.1f);
            
            _currentAngleSlot = GetSlotByAngle(angle);

            if (findTarget)
            {
                OnTargetFound?.Invoke();
            }

            HasActiveTarget = true;
        }

        private int GetSlotByAngle(float angle)
        {
            int slot = -1;
            float anglePerSlot = 360f / GameParameters.GameplayValues.AngleSlots;
            
            for (int i = 0; i < GameParameters.GameplayValues.AngleSlots; i++)
            {
                var currentAngle = i * anglePerSlot;
                if (Mathf.Approximately(currentAngle, angle))
                {
                    slot = i;
                    break;
                }
            }
            
            return slot;
        }
        
        int GetSlotFromPosition(Vector2 from, Vector2 to)
        {
            var totalSlots = GameParameters.GameplayValues.AngleSlots;
            Vector2 dir = to - from;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // ángulo en grados
            if (angle < 0) angle += 360f; // normalizamos 0-360°
            
            int slot = Mathf.FloorToInt(angle / (360f / totalSlots)) % totalSlots;
            return slot;
        }


        public int GetMeteorAngleSlot()
        {
            return _currentAngleSlot;
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

        public int GetDirectionToMeteor(Vector2 shieldPosition, float shieldAngle)
        {
            if (_activeTarget == null) return 0;
            
            float anglePerSlot = 360f / GameParameters.GameplayValues.AngleSlots;
            Vector2 direction = _activeTarget.Position - shieldPosition;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var delta= Mathf.DeltaAngle(shieldAngle + anglePerSlot, targetAngle);
            const float tolerance = 0.3f;
            
            return Mathf.Abs(delta) < tolerance ? 0 : (int)Mathf.Sign(delta);
        }

        private void Target_OnDeathHandler()
        {
            _activeTarget.OnDeath -= Target_OnDeathHandler;
            _activeTarget = null;
            _currentAngleSlot = -1;
            HasActiveTarget = false;
            
            OnTargetLost?.Invoke();
        }
    }
}