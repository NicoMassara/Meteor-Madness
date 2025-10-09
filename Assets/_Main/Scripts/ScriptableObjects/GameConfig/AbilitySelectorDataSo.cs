using System;
using _Main.Scripts.InspectorTools;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects.GameConfig
{
    [CreateAssetMenu(fileName = "SO_AbilityData_Name", menuName = "Scriptable Objects/Game Config/Ability/Selector Data", order = -1)]
    public class AbilitySelectorDataSo : ScriptableObject, IAbilitySelector
    {
        [ReadOnly]
        public int minUnlockLevel;

        private int _maxLevel;
        public int MinUnlockLevel => minUnlockLevel;

        private void UpdateMinUnlockLevel()
        {
            int level = int.MaxValue;
                
            for (int i = 0; i < levelToUnlock.Length; i++)
            {
                if (levelToUnlock[i].levelToSet < level)
                {
                    level = levelToUnlock[i].levelToSet;
                }
            }
                
            minUnlockLevel = level;
        }
        
        [Space]
        [SerializeField] private RarityValues[] rarityValues;
        
        [Serializable]
        private class RarityValues 
        {
            [ReadOnly]
            public AbilityType type;
            [Tooltip("Higher is common, lower is uncommon, Zero removes it")]
            [Range(0,10)]
            public int value;
        }

        private void SetRarityDefault(int abilityCount)
        {
            for (int i = 0; i < abilityCount; i++)
            {
                rarityValues[i] = new RarityValues
                {
                    type = (AbilityType)i+1,
                    value = 10
                };
            }
        }

        public Tuple<AbilityType[], int[]> GetRarityValues()
        {
            var length = rarityValues.Length;
            var tempItem1 = new AbilityType[length];
            var tempItem2 = new int[length];
        
            for (int i = 0; i < length; i++)
            {
                tempItem1[i] = rarityValues[i].type;
                tempItem2[i] = rarityValues[i].value;
            }
        
            return new Tuple<AbilityType[], int[]>(tempItem1, tempItem2);
        }

        [Space]
        [SerializeField] private LevelUnlock[] levelToUnlock;

        [Serializable]
        private class LevelUnlock
        {
            public AbilityType type;
            [Tooltip("DO NOT REPEAT LEVELS")]
            [Min(1)]
            public int levelToSet;
        }
        
        private void SetLevelToUnlockLimit(int levelAmount)
        {
            for (int i = 0; i < levelToUnlock.Length; i++)
            {
                if (levelToUnlock[i].levelToSet > levelAmount)
                {
                    levelToUnlock[i].levelToSet = levelAmount;
                }
            }
        }
        
        public Tuple<int[],AbilityType[]> GetUnlockLevelValues()
        {
            var length = levelToUnlock.Length;
            var tempItem1 = new int[length];
            var tempItem2 = new AbilityType[length];
        
            for (int i = 0; i < length; i++)
            {
                tempItem1[i] = levelToUnlock[i].levelToSet;
                tempItem2[i] = levelToUnlock[i].type;
            }
        
            return  new Tuple<int[], AbilityType[]>(tempItem1, tempItem2);
        }


        private void LimitMaxLevel()
        {
            for (int i = 0; i < levelToUnlock.Length; i++)
            {
                var item = levelToUnlock[i];
                if (item.levelToSet >= _maxLevel)
                {
                    item.levelToSet = _maxLevel-1;
                }
            }
        }


        private void OnValidate()
        {
            //Substracts 1 to ignore Ability.None

            int abilityCount = (int)AbilityType.Default_MAX-1;
            GameConfigUtilities.UpdateArray(ref rarityValues, abilityCount, SetRarityDefault);
            UpdateMinUnlockLevel();
            LimitMaxLevel();
        }

        public void ValidateByLevelAmount(int levelAmount)
        {
            _maxLevel = levelAmount;
            SetLevelToUnlockLimit(levelAmount);
        }
    }
}