using System;
using UnityEngine;

namespace MeteorMadness.GlobalValues.Utilities
{
    public struct AngleCalculations
    {
        public static Vector2 GetPositionByAngle(float angle, float radius)
        {
            float radians = angle * Mathf.Deg2Rad;
            
            //Point in Radius
            Vector2 point = new Vector2(MathF.Cos(radians), Mathf.Sin(radians)) * radius;
            return point;
        }
        
        public static int GetSlotDistance(int currentSlot, int targetSlot, int slotCount = 32)
        {
            int diff = Math.Abs(currentSlot - targetSlot);
            return Math.Min(diff, slotCount - diff);
        }

        public static int GetDirectionFromSlotToTargetSlot(int targetSlot, int originSlot, int slotCount = 32)
        {
            int diff = targetSlot - originSlot;

            if (diff > slotCount / 2) diff -= slotCount;
            if (diff < -slotCount / 2) diff += slotCount;
            return Math.Sign(diff);
        }
        
        public static int GetDirectionFromAngleToSlot(int target, float origin, int slotCount = 32, float angleOffset = 0)
        {
            int originSlot = GetSlotFromAngle(origin, slotCount, angleOffset);
            return GetDirectionFromSlotToTargetSlot(target,originSlot,slotCount);
        }

        public static float GetSlotSize(int angleSlots = 32) => (360f) / angleSlots;

        public static int GetSlotFromAngle(float angle, int angleSlots = 32, float angleOffset = 0f)
        {
            if (angleSlots <= 0)
                throw new ArgumentException("angleSlots must be greater than 0");

            float normalized = Mathf.Repeat(angle + angleOffset, 360f);
            float step = 360f / angleSlots;

            return Mathf.FloorToInt(normalized / step);
        }
        
        public static float GetAngleFromSlot(int slot, int angleSlots = 32, float angleOffset = 0)
        {
            var angle = (slot * GetSlotSize(angleSlots)) + angleOffset;
            return (angle + 360f) % 360f;
        }
    }
}