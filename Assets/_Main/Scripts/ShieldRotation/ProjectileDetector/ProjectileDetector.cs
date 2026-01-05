using System;
using _Main.Scripts.ShieldRotation.Contracts;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.ProjectileDetector
{
    public class ProjectileDetector : IProjectileDetector
    {
        private readonly Collider2D[] _colliders;
        private readonly LayerMask _targetLayerMask;
        
        public ProjectileDetector(IMovement movement, LayerMask targetLayerMask)
        {
            _targetLayerMask = targetLayerMask;
            _colliders = new Collider2D[10];
        }

        public ITargetable GetNearestTarget(Vector2 movementPosition, float checkRadius = Mathf.Infinity)
        {
            var filter = new ContactFilter2D
            {
                layerMask = _targetLayerMask,
                useLayerMask = true
            };
            
            var hitCount = Physics2D.OverlapCircle(movementPosition, checkRadius, filter,_colliders);
            
            return hitCount == 0 ? 
                null : 
                _colliders[GetNearestProjectile(movementPosition, hitCount)].GetComponent<ITargetable>();
        }
        
        private int GetNearestProjectile(Vector2 movementPosition, int hitCount)
        {
            var minDistance = float.MaxValue;
            var selectedIndex = -1;
            
            for (int i = 0; i < hitCount; i++)
            {
                var item = _colliders[i].GetComponent<ITargetable>();
                if (item == null) continue;
                if (item.CanBeTargeted == false) continue;
                var distance = Vector2.Distance(item.Position, movementPosition);

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