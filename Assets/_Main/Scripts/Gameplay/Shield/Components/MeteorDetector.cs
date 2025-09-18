using System;
using _Main.Scripts.Gameplay.Meteor;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Shield
{
    public class MeteorDetector
    {
        private readonly float _checkRadius;
        private readonly LayerMask _meteorLayer;
        private readonly Transform _shieldTransform;
        private bool _hasMeteor;
        private IMeteor _activeMeteor;
        
        private Collider2D[] _colliders = new Collider2D[10];
        private MeteorView _meteorView;
        
        public UnityAction<MeteorDetectedData> OnMeteorDetected;

        public MeteorDetector(Transform shieldTransform, float checkRadius, LayerMask meteorLayer)
        {
            _shieldTransform = shieldTransform;
            _checkRadius = checkRadius;
            _meteorLayer = meteorLayer;
        }

        public void CheckForNearMeteor()
        {
            if(_hasMeteor) return;
            
            var hitCount = Physics2D.OverlapCircleNonAlloc(_shieldTransform.position, _checkRadius, _colliders,_meteorLayer);

            if (hitCount == 0)
            {
                return;
            }

            _hasMeteor = true;
            _activeMeteor = _colliders[GetNearestMeteor(hitCount)].GetComponent<IMeteor>();

            _activeMeteor.OnDeflection += OnMeteorCollision;
            _activeMeteor.OnEarthCollision += OnMeteorCollision;
            
            var meteorPos = _activeMeteor.Position;
            var dir = (meteorPos - (Vector2)_shieldTransform.position).normalized;
            var angle = Mathf.Repeat((Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg), 360f);
            
            Debug.DrawRay(_shieldTransform.position, dir * _checkRadius, Color.magenta, 0.1f);
            
            OnMeteorDetected?.Invoke(new MeteorDetectedData
            {
                Angle = angle,
                Direction = dir,
            });
        }

        private void OnMeteorCollision(MeteorCollisionData data)
        {
            _activeMeteor.OnDeflection -= OnMeteorCollision;
            _activeMeteor.OnEarthCollision -= OnMeteorCollision;
            _hasMeteor = false;
        }

        private int GetNearestMeteor(int hitCount)
        {
            var minDistance = float.MaxValue;
            var shieldPos = _shieldTransform.position;
            var selectedIndex = -1;
            
            for (int i = 0; i < hitCount; i++)
            {
                var meteorPos = _colliders[i].transform.position;
                var distance = Vector2.Distance(meteorPos, shieldPos);

                if (distance < minDistance)
                {
                    selectedIndex = i;
                    minDistance = distance;
                }
            }

            return selectedIndex;
        }
    }

    public class MeteorDetectedData
    {
        public float Angle;
        public Vector2 Direction;
    }
}