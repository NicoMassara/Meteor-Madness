using System;
using UnityEngine;

namespace MeteorMadness.GlobalValues.Utilities
{
    public struct AngleCalculations
    {
        public static float GetAngleBySlot(int selectedSlot, int slotAmount)
        {
            float anglePerSlot = 360f / slotAmount;
            return selectedSlot * anglePerSlot;
        }
        
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
        
        public static float GetSlotSize(int angleSlots = 32) => (360f) / angleSlots;
        
        public static float GetAngleFromSlot(int slot, int angleSlots = 32, float angleOffset = 0)
        {
            var angle = (slot * GetSlotSize(angleSlots)) + angleOffset;
            return (angle + 360f) % 360f;
        }
    }
}