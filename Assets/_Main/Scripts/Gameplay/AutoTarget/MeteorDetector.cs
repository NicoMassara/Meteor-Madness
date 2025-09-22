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
        private Func<float> _getShieldAngle;
        private Vector2 _shieldPosition = Vector2.zero;
        private ITargetable _activeMeteor;
        private float _currentAngle;
        
        public UnityAction OnTargetLost;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;


        public MeteorDetector(Vector2 shieldPosition, Func<float> angleGetter, LayerMask meteorLayer)
        {
            _shieldPosition = shieldPosition;
            _getShieldAngle = angleGetter;
            _meteorLayer = meteorLayer;
        }
        
        public void CheckForNearMeteor(float checkRadius)
        {
            var hitCount = Physics2D.OverlapCircleNonAlloc(_shieldPosition, checkRadius, _colliders,_meteorLayer);

            if (hitCount == 0)
            {
                return;
            }
            
            _activeMeteor = _colliders[GetNearestMeteor(hitCount)].GetComponent<ITargetable>();

            _activeMeteor.OnDeath += Target_OnDeathHandler;
            
            var meteorPos = _activeMeteor.Position;
            var dir = (meteorPos - _shieldPosition).normalized;
            var angle = Mathf.Repeat(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, 360f);
            
            Debug.DrawRay(_shieldPosition,
                dir * checkRadius, Color.magenta, 0.1f);
            
            _currentAngle = angle;
        }
        

        public int GetDirectionToMeteorAngle()
        {
            if (_activeMeteor == null)
            {
                return 0;
            }

            var delta = Mathf.DeltaAngle(_getShieldAngle(), _currentAngle);
            
            const float tolerance = 0.5f; // degrees

            if (Mathf.Abs(delta) <= tolerance)
                return 0;

            return delta > 0 ? 1 : -1;
        }

        private void Target_OnDeathHandler()
        {
            _activeMeteor.OnDeath -= Target_OnDeathHandler;
            _activeMeteor = null;
            
            OnTargetLost?.Invoke();
        }

        private int GetNearestMeteor(int hitCount)
        {
            var minDistance = float.MaxValue;
            var selectedIndex = -1;
            
            for (int i = 0; i < hitCount; i++)
            {
                var meteorPos = _colliders[i].transform.position;
                var distance = Vector2.Distance(meteorPos, _shieldPosition);

                if (distance < minDistance)
                {
                    selectedIndex = i;
                    minDistance = distance;
                }
            }

            return selectedIndex;
        }
    }
}