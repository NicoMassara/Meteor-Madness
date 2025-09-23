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
        private ITargetable _activeTarget;
        private float _currentAngle;
        public bool HasActiveTarget { get; private set; }

        public UnityAction OnTargetLost;

        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Shield;


        public MeteorDetector(Func<float> angleGetter, LayerMask meteorLayer)
        {
            _getShieldAngle = angleGetter;
            _meteorLayer = meteorLayer;
        }
        
        public void CheckForNearMeteor(Vector2 position, float checkRadius)
        {
            if(HasActiveTarget) return;
            
            var hitCount = Physics2D.OverlapCircleNonAlloc(position, checkRadius, _colliders,_meteorLayer);

            if (hitCount == 0)
            {
                return;
            }
            
            _activeTarget = _colliders[GetNearestMeteor(position, hitCount)].GetComponent<ITargetable>();

            _activeTarget.OnDeath += Target_OnDeathHandler;
            
            var meteorPos = _activeTarget.Position;
            var dir = (meteorPos - position).normalized;
            var angle = Mathf.Repeat(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, 360f);
            
            Debug.DrawRay(position,
                dir * checkRadius, Color.magenta, 0.1f);
            
            _currentAngle = angle;
            HasActiveTarget = true;
        }
        

        public int GetDirectionToMeteorAngle()
        {
            if (_activeTarget == null)
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
            _activeTarget.OnDeath -= Target_OnDeathHandler;
            _activeTarget = null;
            HasActiveTarget = false;
            
            OnTargetLost?.Invoke();
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
    }
}