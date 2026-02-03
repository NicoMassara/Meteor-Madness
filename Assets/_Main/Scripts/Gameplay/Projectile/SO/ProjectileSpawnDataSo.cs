using System;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.SO
{
    [CreateAssetMenu(fileName = "So_ProjectileSpawnData_Default", menuName = "Scriptable Objects/Projectile Spawn/Spawn Data", order = 0)]
    public class ProjectileSpawnDataSo : ScriptableObject, IProjectileSpawnData
    {
        #region Data Range
        
        [System.Serializable]
        private class FloatRangeData : IFloatRangeData
        {
            [Min(0)] public float minRange = 0;
            [Min(0)] public float maxRange = 0;
            
            public Vector2 Range => new Vector2(minRange, maxRange);
            public float RandomRange => GetRandomRange();

            private float GetRandomRange()
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                return minRange == maxRange ? minRange : RandomService.Range(minRange, maxRange);
            }
        }
        
        [System.Serializable]
        private class DistanceRatioRangeData : IFloatRangeData
        {
            [Range(0, 1)] public float minRange = 1;
            [Range(0, 1)] public float maxRange = 1;
            
            public Vector2 Range => new Vector2(minRange, maxRange);
            public float RandomRange => GetRandomRange();

            private float GetRandomRange()
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                return minRange == maxRange ? minRange : RandomService.Range(minRange, maxRange);
            }
        }
        
        [System.Serializable]
        private class AmountRangeData : IIntRangeData
        {
            [Min(1)] public int minRange = 1;
            [Min(1)] public int maxRange = 1;
            
            public Vector2Int Range => new Vector2Int(minRange, maxRange);
            public int RandomRange => GetRandomRange();

            private int GetRandomRange()
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                return minRange == maxRange ? minRange : RandomService.Range(minRange, maxRange + 1);
            }
        }
        
        [System.Serializable]
        private class SlotRangeData : ISlotRangeData
        {
            [Range(1, GameParameters.GameplayValues.AngleSlots)]
            public int minRange = 1;

            [Range(1, GameParameters.GameplayValues.AngleSlots)]
            public int maxRange = 1;
            
            public Vector2Int Range => new Vector2Int(minRange, maxRange);
            public int RandomRange => GetRandomRange();

            private int GetRandomRange()
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                return minRange == maxRange ? minRange : RandomService.Range(minRange, maxRange + 1);
            }
        }
        
        [System.Serializable]
        public class EnumRangeData<T> : IEnumRangeData<T> 
            where T : Enum 
        {
            public T minRange;
            public T maxRange;

            public Vector2Int Range => new (Convert.ToInt32(minRange), Convert.ToInt32(maxRange));

            public T RandomRange => GetRandomRange();

            private T GetRandomRange()
            {
                int min = Convert.ToInt32(minRange);
                int max = Convert.ToInt32(maxRange);
                
                if (min == max) return minRange;
                
                return (T)(object)RandomService.Range(min, max);
            }
        }
        
        #endregion

        [System.Serializable]
        private class BatchData : IBatchSpawnData
        {
            [SerializeField] private AmountRangeData projectileAmountRange;
            [SerializeField] private SlotRangeData slotRange;
            [Tooltip("0f - Spawn Point / 1f - Shield")]
            [SerializeField] private DistanceRatioRangeData innerBatchDistanceRange;
            [Tooltip("0f - Spawn Point / 1f - Shield")]
            [SerializeField] private DistanceRatioRangeData nextBatchDistanceRange;
            [SerializeField] private FloatRangeData nextBatchDelayRange;
            [SerializeField] private SlotRangeData nextBatchSlotRange;
            [SerializeField] private EnumRangeData<SpawnType> spawnTypeRange;

            public IIntRangeData ProjectileAmountRange => projectileAmountRange;
            public ISlotRangeData SlotRange => slotRange;
            public IFloatRangeData InnerBatchDistanceRange => innerBatchDistanceRange;
            public IFloatRangeData NextBatchDistanceRange => nextBatchDistanceRange;
            public IFloatRangeData NextBatchDelayRange => nextBatchDelayRange;
            public ISlotRangeData NextBatchSlotRange => nextBatchSlotRange;
            public IEnumRangeData<SpawnType> SpawnTypeRange => spawnTypeRange;
        }
        
        [SerializeField] private BatchData[] batchData;

        public IBatchSpawnData GetDataByIndex(int index) => batchData[index];

        private void OnValidate()
        {
            SetBatchArray();
        }
        
        private void OnEnable()
        {
            SetBatchArray();
        }

        private void SetBatchArray()
        {
            if (batchData == null || batchData.Length != GameParameters.GameplayValues.SpawnLevelAmount)
            {
                batchData = new BatchData[GameParameters.GameplayValues.SpawnLevelAmount];
            }
        }
    }
}