using System;
using System.Collections.Generic;
using _Main.Scripts.InspectorTools;
using _Main.Scripts.Interfaces;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_GameConfig_Name", menuName = "Scriptable Objects/Game Config/Game Data", order = 0)]
    public class GameplayConfigSo : ScriptableObject
    {
        [Header("Values")]
        [Range(1,25)]
        [SerializeField] private int levelAmount;
        [Range(10,500)]
        [SerializeField] private int pointsMultiplier;
        [Space]

        #region Level Data

        [SerializeField] private GameLevelData levelData;
        
        [Serializable]
        public class GameLevelData : IGameLevelData
        {
            [Space] 
            [Tooltip("Meteor deflect count needed to increase each internal level")] 
            [Range(1, 100)]
            [SerializeField] private int[] gameplayLevelRequierment;

            public int[] GetGameplayLevelRequierment()
            {
                return gameplayLevelRequierment;
            }

            public void ValidateByLevel(int levelAmount)
            {
                UpdateArray(ref gameplayLevelRequierment, levelAmount);
            }

            private void UpdateArray<T>(ref T[] array, int maxSize)
            {
                if (array == null || array.Length != maxSize)
                {
                    var oldArray = array;
                    array = new T[maxSize];

                    // Copiar valores existentes para no perder referencias
                    if (oldArray != null)
                    {
                        for (int i = 0; i < Mathf.Min(oldArray.Length, array.Length); i++)
                        {
                            array[i] = oldArray[i];
                        }
                    }
                }
            }
        }

        #endregion

        #region Ability Selector

        [SerializeField] private GameAbilitySelector abilitySelector;

        [Serializable]
        public class GameAbilitySelector : IGameAbilitySelector
        {
            [ReadOnly]
            public int minUnlockLevel;
            
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

            public (AbilityType[] abilityTypes, int[] rarityValues) GetRarityValues()
            {
                var length = rarityValues.Length;
                var tempItem1 = new AbilityType[length];
                var tempItem2 = new int[length];
            
                for (int i = 0; i < length; i++)
                {
                    tempItem1[i] = rarityValues[i].type;
                    tempItem2[i] = rarityValues[i].value;
                }
            
                return  (tempItem1, tempItem2);
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
            
            public (int[] unlockLevels, AbilityType[] abilityTypes) GetUnlockLevelValues()
            {
                var length = levelToUnlock.Length;
                var tempItem1 = new int[length];
                var tempItem2 = new AbilityType[length];
            
                for (int i = 0; i < length; i++)
                {
                    tempItem1[i] = levelToUnlock[i].levelToSet;
                    tempItem2[i] = levelToUnlock[i].type;
                }
            
                return  (tempItem1, tempItem2);
            }
            
            
            public void Validate(int levelAmount)
            {
                //Substracts 1 to ignore Ability.None
                int abilityCount = (int)AbilityType.Default_MAX-1;
                GameConfigUtilities.UpdateArray(ref rarityValues, abilityCount, SetRarityDefault);
                UpdateMinUnlockLevel();
                SetLevelToUnlockLimit(levelAmount);
            }
        }

        #endregion

        #region Projectile Data

        [SerializeField] private GameProjectileData projectileData;
        
        [System.Serializable]
        public class GameProjectileData : IGameProjectileData
        {
            [Range(1, 50)] 
            [SerializeField] private int maxProjectileSpeed;
            
            public int MaxProjectileSpeed => maxProjectileSpeed;
            
            
            [Space]
            [SerializeField] private TravelMultiplier[] travelMultiplier;
            
            [Serializable]
            private class TravelMultiplier
            {
                [Range(0.1f,2)]
                public float speedMultiplier;
                [Tooltip("0 = Earth, 1 = Spawn")]
                [Range(0,1f)]
                public float travelRatio;
            }

            public (float[] speed, float[] travel) GetTravelData()
            {
                var lenght = slotDistanceData.Length;
                
                var speedTemp = new float[lenght];
                var travelTemp = new float[lenght];
                
                for (int i = 0; i < lenght; i++)
                {
                    speedTemp[i] = travelMultiplier[i].speedMultiplier;
                    travelTemp[i] = travelMultiplier[i].travelRatio;
                }
                
                return (speedTemp, travelTemp);
            }

            private void SetTravelLimits()
            {
                for (int i = 1; i < travelMultiplier.Length; i++)
                {
                    if (travelMultiplier[i].travelRatio < travelMultiplier[i - 1].travelRatio)
                    {
                        travelMultiplier[i].travelRatio = travelMultiplier[i - 1].travelRatio; // clamp upward
                    }
                
                    if (travelMultiplier[i].speedMultiplier < travelMultiplier[i - 1].speedMultiplier)
                    {
                        travelMultiplier[i].speedMultiplier = travelMultiplier[i - 1].speedMultiplier; // clamp upward
                    }
                }
            }

            [SerializeField] private SlotDistanceData[] slotDistanceData;
            
            [Serializable]
            private class SlotDistanceData
            {
                [Range(1, GameParameters.GameplayValues.AngleSlots)]
                public int minSlotProximity;
                [Range(2, GameParameters.GameplayValues.AngleSlots)] 
                public int maxSlotProximity;
            }
            
            private void SetSlotLimits()
            {
                for (int i = 1; i < travelMultiplier.Length; i++)
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
            

            public void Validate(int levelAmount)
            {
                GameConfigUtilities.UpdateArray(ref travelMultiplier, levelAmount);
                GameConfigUtilities.UpdateArray(ref slotDistanceData, levelAmount);
                SetTravelLimits();
                SetSlotLimits();
            }
        }

        #endregion

        #region Getters

        public int LevelAmount => levelAmount;

        public int PointsMultiplier => pointsMultiplier;
        
        public IGameLevelData LevelData => levelData;

        public IGameAbilitySelector AbilitySelector => abilitySelector;

        public IGameProjectileData ProjectileData => projectileData;

        #endregion
        
        private void OnValidate()
        {
            abilitySelector.Validate(levelAmount);
            levelData.ValidateByLevel(levelAmount);
            projectileData.Validate(levelAmount);
        }
    }
}