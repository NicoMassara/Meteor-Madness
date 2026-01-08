using System;
using _Main.Scripts.ShieldRotation.Contracts;
using _Main.Scripts.ShieldRotation.Tools;
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
            //_angleOffset = 180f;
        }

        public float GetDistanceToTarget(ITargetable target)
        {
            return Vector2.Distance(target.Position, _center.position);
        }
        public int GetDirectionToTarget(ITargetable target)
        {
            return AngleHelper.GetDirectionToTarget(
                GetAngleSlotFromTarget(target), _movementComponent.GetCurrentSlot(),
                _slotCount);
        }

        public int GetSlotDistance(ITargetable target)
        {
            int current = _movementComponent.GetCurrentSlot();
            int targetSlot = GetAngleSlotFromTarget(target);
            
            return AngleHelper.GetSlotDistance(current, targetSlot, _slotCount);
        }

        public bool GetTargetIsInRange(ITargetable target)
        {
            var slotDistance = GetSlotDistance(target);
            var distance = GetDistanceToTarget(target);

            if (distance > _data.MaxDistance)
            {
                return false;
            }
            
            if (slotDistance > _data.CorrectionSlotDistance)
            {
                return false;
            }

            return true;
        }
        

        public int GetAngleSlotFromTarget(ITargetable target)
        {
            if (target == null) return -1;
            
            return AngleHelper.GetAngleSlotFromPosition(target.Position, _center.position, 
                _slotCount, _angleOffset);
        }
    }
}