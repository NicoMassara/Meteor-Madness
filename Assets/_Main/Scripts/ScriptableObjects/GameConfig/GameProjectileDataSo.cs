using System;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_ProjectileData_Name", menuName = "Scriptable Objects/Game Config/Projectile Data", order = 0)]
    public class GameProjectileDataSo : ScriptableObject, IGameProjectileData
    {
        [Header("Movement")]
        [Range(1, 50)] 
        [SerializeField] private int maxProjectileSpeed;
        [Space]
        [SerializeField] private AnimationCurveData speedMultiplier;
        [Space]
        [SerializeField] private AnimationCurveData travelRatioCurve;
        [Space]
        [Space]
        [Header("Slots")]
        [SerializeField] private SlotDistanceData[] slotDistanceData;
        
        public float MaxProjectileSpeed => maxProjectileSpeed;

        public float GetSpeedMultiplier(float index)
        {
            return speedMultiplier.GetCurveValue(index);
        }

        public float GetTravelRatio(float curveValue)
        {
            return travelRatioCurve.GetCurveValue(curveValue);
        }

        public (int[] minSlot, int[] maxSlot) GetSlotData()
        {
            var lenght = slotDistanceData.Length;
            
            var tempMin = new int[lenght];
            var tempMax = new int[lenght];
            
            for (int i = 0; i < lenght; i++)
            {
                tempMin[i] = slotDistanceData[i].minSlotProximity;
                tempMax[i] = slotDistanceData[i].maxSlotProximity;
            }
            
            return (tempMin, tempMax);
        }

        [Serializable]
        private class AnimationCurveData
        {
            [SerializeField] private AnimationCurve animationCurve;
            [Range(0,1f)]
            [SerializeField] private float minValue = .35f;
            [Range(0,1f)]
            [SerializeField] private float maxValue = 1f;

            public float GetCurveValue(float input)
            {
                var value = animationCurve.Evaluate(input);
                return Mathf.Lerp(minValue, maxValue, value);
            }

            public void ValidateCurve()
            {
                if (animationCurve.length <= 1)
                {
                    animationCurve.ClearKeys();
                    animationCurve.AddKey(new Keyframe(0, 0));
                    animationCurve.AddKey(new Keyframe(1, 1));
                }
            }
        }

        [Serializable]
        private class SlotDistanceData
        {
            [Range(1, GameParameters.GameplayValues.AngleSlots)]
            public int minSlotProximity;
            [Range(2, GameParameters.GameplayValues.AngleSlots)] 
            public int maxSlotProximity;
        }
        

        #region Limiters
        
        private void SetSlotLimits()
        {
            if(slotDistanceData == null) return;
            
            for (int i = 1; i < slotDistanceData.Length; i++)
            {
                if (slotDistanceData[i].minSlotProximity >= slotDistanceData[i].maxSlotProximity)
                {
                    slotDistanceData[i].minSlotProximity = slotDistanceData[i].maxSlotProximity-1;
                }

                if (slotDistanceData[i].maxSlotProximity <= slotDistanceData[i].minSlotProximity)
                {
                    slotDistanceData[i].maxSlotProximity = slotDistanceData[i].minSlotProximity + 1;
                }
            }
            
        }

        #endregion

        private void OnValidate()
        {
            SetSlotLimits();
            speedMultiplier.ValidateCurve();
            travelRatioCurve.ValidateCurve();
        }

        public void ValidateByLevelAmount(int levelAmount)
        {
            GameConfigUtilities.UpdateArray(ref slotDistanceData, levelAmount);
        }
    }
}