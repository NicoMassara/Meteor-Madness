using System;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Utilities
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
    }
}