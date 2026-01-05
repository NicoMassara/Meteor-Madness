using System;
using _Main.Scripts.ShieldRotation.Contracts;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.MovementCorrection
{
    public class MovementCorrectionComponent : IMovementCorrection
    {
        private readonly IMovementCorrectionData _data;
        private readonly Transform _center;
        private readonly IMovement _movementComponent;
        private readonly LayerMask _targetLayerMask;
        private readonly int _slotCount;
        private readonly float _angleOffset;

        public MovementCorrectionComponent(IMovementCorrectionData data, IMovement movement, Transform center, int slotCount)
        {
            _data = data;
            _movementComponent = movement;
            _center = center;
            _slotCount = slotCount;
            _angleOffset = 180f;
        }

        public float GetDistanceToTarget(ITargetable target)
        {
            return Vector2.Distance(target.Position, _center.position);
        }
        public int GetDirectionToTarget(ITargetable target)
        {
            int diff = GetAngleSlotFromTarget(target) - _movementComponent.GetCurrentSlot();

            if (diff > _slotCount / 2) diff -= _slotCount;
            if (diff < -_slotCount / 2) diff += _slotCount;

            return Math.Sign(diff);
        }

        public int GetSlotDistance(ITargetable target)
        {
            int current = _movementComponent.GetCurrentSlot();
            int targetSlot = GetAngleSlotFromTarget(target);

            int diff = Math.Abs(current - targetSlot);
            return Math.Min(diff, _slotCount - diff);
        }

        public bool GetTargetIsInRange(ITargetable target)
        {
            return GetSlotDistance(target) <= _data.CorrectionSlotDistance && 
                   GetDistanceToTarget(target) <= _data.MaxDistance;
        }

        public bool GetIsInFrontOfTarget(ITargetable target)
        {
            return GetSlotDistance(target) == 0 &&
                   GetDistanceToTarget(target) <= _data.MaxDistance;
        }

        public int GetAngleSlotFromTarget(ITargetable target)
        {
            if (target == null) return -1;

            var dir = target.Position - (Vector2)_center.position;
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - _angleOffset;
            angle = (angle + 360f) % 360f;
            
            int slot = Mathf.FloorToInt(angle / (360f / _slotCount)) % _slotCount;
            return slot;
        }
    }
}