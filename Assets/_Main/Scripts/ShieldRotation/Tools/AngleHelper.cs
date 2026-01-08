using System;
using UnityEngine;

namespace _Main.Scripts.ShieldRotation.Tools
{
    public class AngleHelper
    {
        public static int GetSlotDistance(int currentSlot, int targetSlot, int slotCount = 32)
        {
            int diff = Math.Abs(currentSlot - targetSlot);
            return Math.Min(diff, slotCount - diff);
        }
        
        public static float GetAngularDistanceCCW(float origin, float target)
        {
            return Mathf.Repeat(target - origin, 360f);
        }
        
        public static float GetAngularDistanceCW(float origin, float target)
        {
            return Mathf.Repeat(origin - target, 360f);
        }
        
        public static float SignedAngularDistance(float origin, float target)
        {
            return Mathf.Repeat(target - origin + 180f, 360f) - 180f;
        }
        
        public static int GetAngleSlotFromPosition(Vector2 originPosition, Vector2 targetPosition, int slotCount = 32, float angleOffset = 180f)
        {
            var dir = targetPosition - originPosition;
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - angleOffset;
            angle = (angle + 360f) % 360f;
            
            int slot = Mathf.FloorToInt(angle / (360f / slotCount)) % slotCount;
            return slot;
        }
        
        public static int GetDirectionToTarget(int targetSlot, int originSlot, int slotCount = 32)
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