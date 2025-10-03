using System;
using _Main.Scripts.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay.Projectile
{
    public class ProjectileSpawnLocationController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform centerOfGravity;
        [Header("Values")] 
        [Range(22f,100f)]
        [SerializeField] private float spawnRadius;
        [Range(1, SlotAmount)]
        [SerializeField] private int minSlotProximity;
        [Range(2, SlotAmount)] 
        [SerializeField] private int maxSlotProximity;
        

        private const int SlotAmount = GameParameters.GameplayValues.AngleSlots;
        private int _lastSlot;
        
        private void OnValidate()
        {
            if (minSlotProximity >= maxSlotProximity)
            {
                minSlotProximity = maxSlotProximity-1;
            }

            if (maxSlotProximity <= minSlotProximity)
            {
                maxSlotProximity = minSlotProximity + 1;
            }
        }


        private void Awake()
        {
            GameManager.Instance.EventManager.Subscribe<GameModeEvents.Start>(EventBus_OnGameModeStart);
        }

        #region Create Spawn Angle

        // ReSharper disable Unity.PerformanceAnalysis
        public float GetSpawnAngle()
        {
            int selectedSlot = GetValidSlot(_lastSlot);
            var angle = GetAngleBySlot(selectedSlot);
            
            _lastSlot = selectedSlot;
            return angle;
        }
        
        private int GetValidSlot(int currentSlot)
        {
            int slot = 0;
            int safetyCounter = 0;

            do
            {
                slot = Random.Range(0, SlotAmount);
                
                safetyCounter++;
                if (safetyCounter > 100)
                {
                    Debug.LogWarning("Safety Counter Exceeded");
                    slot = maxSlotProximity/2;
                    break;
                }

            } while (IsValidSlot(slot,currentSlot) == false); 
            
            return slot;
        }

        private bool IsValidSlot(int selectedSlot, int lastSlot)
        {
            var diff = Mathf.Abs(selectedSlot - lastSlot);
            
            return diff >= minSlotProximity && diff <= maxSlotProximity;
        }

        private float GetAngleBySlot(int selectedSlot)
        {
            float anglePerSlot = 360f / SlotAmount;
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

        public Vector2 GetCenterOfGravity()
        {
            return centerOfGravity.position;
        }
        
        private void RestartValues()
        {
            _lastSlot = 0;
        }

        public float GetSpawnRadius()
        {
            return spawnRadius;
        }

        #region Event Bus

        private void EventBus_OnGameModeStart(GameModeEvents.Start input)
        {
            RestartValues();
        }
        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius);
            for (int i = 0; i < SlotAmount; i++)
            {
                var angle = GetAngleBySlot(i);
                Gizmos.DrawLine(centerOfGravity.position, GetPositionByAngle(angle, spawnRadius));
            }
        }

        #endregion
    }
}