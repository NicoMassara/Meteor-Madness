using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileAngleController
    {
        private int _lastSlot;
        private readonly int _slotAmount;

        public ProjectileAngleController(int slotAmount)
        {
            _slotAmount = slotAmount;
        }

        #region Create Spawn Angle

        // ReSharper disable Unity.PerformanceAnalysis
        public float GetSpawnAngle(int minSlotProximity, int maxSlotProximity)
        {
            int selectedSlot = GetValidSlot(_lastSlot, minSlotProximity, maxSlotProximity);
            var angle = GetAngleBySlot(selectedSlot);
            
            _lastSlot = selectedSlot;
            return angle;
        }
        
        private int GetValidSlot(int currentSlot, int minSlotProximity, int maxSlotProximity)
        {
            int slot = 0;
            int safetyCounter = 0;

            do
            {
                slot = Random.Range(0, _slotAmount);
                
                safetyCounter++;
                if (safetyCounter > 100)
                {
                    Debug.LogWarning("Safety Counter Exceeded");
                    slot = maxSlotProximity/2;
                    break;
                }

            } while (IsValidSlot(slot,currentSlot, minSlotProximity, maxSlotProximity) == false); 
            
            return slot;
        }

        private bool IsValidSlot(int selectedSlot, int lastSlot, int minSlotProximity, int maxSlotProximity)
        {
            var diff = Mathf.Abs(selectedSlot - lastSlot);
            
            return diff >= minSlotProximity && diff <= maxSlotProximity;
        }

        public float GetAngleBySlot(int selectedSlot)
        {
            float anglePerSlot = 360f / _slotAmount;
            return selectedSlot * anglePerSlot;
        }

        #endregion

        #region Get Position

        //Used by Ring Meteor
        public Vector2 GetPositionByAngle(float angle, float radius)
        {
            float radians = angle * Mathf.Deg2Rad;
            
            //Point in Radius
            Vector2 point = new Vector2(MathF.Cos(radians), Mathf.Sin(radians)) * radius;
            return point;
        }

        #endregion
        
        public void RestartValues()
        {
            _lastSlot = 0;
        }
    }
}