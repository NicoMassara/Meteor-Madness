using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Shield.Rotation.Components
{
    public class TargetDetector : ITargetDetector
    {
        private readonly Collider2D[] _colliders;
        private readonly IDetectorData _data;
        private readonly Transform _originObject;

        public TargetDetector(Transform originObject, IDetectorData data)
        {
            _originObject = originObject;
            _data = data;
            _colliders = new Collider2D[5];
        }

        public bool GetNearestTarget(out int targetSlot)
        {
            targetSlot = -1;
            var filter = new ContactFilter2D
            {
                layerMask = _data.TargetLayerMask,
                useLayerMask = true
            };
            
            var hitCount = Physics2D.OverlapCircle(_originObject.position, _data.CheckRadius, filter,_colliders);

            if (hitCount == 0)
            {
                Debug.LogWarning("A Target could not be found");
                return false;
            }

            var target = GetTargetObject(hitCount);

            if (target == null)
            {
                Debug.LogWarning("A Target could not be found");
                return false;
            }
            
            //target.DisableTargetable();

            targetSlot = target.Slot;
            //Debug.Log($"Target Found, Slot: {targetSlot}");
            return true;
        }
        
        private ITargetable GetTargetObject(int hitCount)
        {
            var minDistance = float.MaxValue;
            ITargetable target = null;
        
            for (int i = 0; i < hitCount; i++)
            {
                var item = _colliders[i].GetComponent<ITargetable>();
                
                if (item == null)
                {
                    Debug.Log("Collider is null");
                    continue;
                }
            
                if (item.CanBeTargeted == false)
                {
                    Debug.Log("Target can't be targeted");
                    continue;
                }

                var distance = Vector2.Distance(item.Position, _originObject.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = item;
                }
            }

            return target;
        }
    }
}