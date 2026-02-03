using System;
using _Main.Scripts.Common.MyRandom;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.SO
{
    public interface IProjectileMilestoneData
    {
        public IIntRangeData GetMilestoneByIndex(int index);
    }
    
    public interface IIntRangeData
    {
        public Vector2Int Range { get; }
        public int RandomRange { get; }
    }
    
    [CreateAssetMenu(fileName = "So_GameModeMilestoneData_Default", menuName = "Scriptable Objects/Game Mode/Milestone Data", order = 0)]
    public class ProjectileMilestoneDataSo : ScriptableObject, IProjectileMilestoneData
    {
        [System.Serializable]
        private class IntRangeData : IIntRangeData
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
        
        [SerializeField] private IntRangeData[] milestonePerLevel;
        
        public IIntRangeData GetMilestoneByIndex(int index) => milestonePerLevel[index];

        private void OnValidate()
        {
            SetMilestoneArray();
        }
        
        private void OnEnable()
        {
            SetMilestoneArray();
        }

        private void SetMilestoneArray()
        {
            if (milestonePerLevel == null || milestonePerLevel.Length != GameParameters.GameplayValues.SpawnLevelAmount)
            {
                milestonePerLevel = new IntRangeData[GameParameters.GameplayValues.SpawnLevelAmount];
            }
        }


    }
}