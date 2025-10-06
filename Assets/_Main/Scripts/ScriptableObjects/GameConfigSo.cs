using System;
using System.Collections.Generic;
using _Main.Scripts.InspectorTools;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_GameConfig_Name", menuName = "Scriptable Objects/Game Config/Game Data", order = 0)]
    public class GameConfigSo : ScriptableObject
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
        public class GameLevelData
        {
            [Space] 
            [Tooltip("Meteor deflect count needed to increase each internal level")] 
            [Range(1, 100)]
            [SerializeField] private int[] gameplayLevelRequierment;
            
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
        public class GameAbilitySelector
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
            
            private void UpdateRarityArray(int abilityCount)
            {
                if (rarityValues == null || rarityValues.Length != abilityCount)
                {
                    var oldArray = rarityValues;
                    rarityValues = new RarityValues[abilityCount];
                    for (int i = 0; i < abilityCount; i++)
                    {
                        rarityValues[i] = new RarityValues
                        {
                            type = (AbilityType)i+1,
                            value = 10
                        };
                    }

                    if (oldArray != null)
                    {
                        for (int i = 1; i < Mathf.Min(oldArray.Length, rarityValues.Length); i++)
                        {
                            rarityValues[i] = oldArray[i];
                        }
                    }
                }
            }

            public Dictionary<AbilityType, int> GetRarityValuesDic()
            {
                var tempDic = new Dictionary<AbilityType, int>();

                for (int i = 0; i < rarityValues.Length; i++)
                {
                    var item = rarityValues[i];
                    if (tempDic.ContainsKey(item.type)) continue;
                    
                    tempDic.Add(item.type, item.value);
                }
                
                return tempDic;
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

            public Dictionary<int, AbilityType> GetLevelToUnlockDic()
            {
                var tempDic = new Dictionary<int, AbilityType>();

                for (int i = 0; i < levelToUnlock.Length; i++)
                {
                    var item = levelToUnlock[i];
                    if (tempDic.ContainsKey(item.levelToSet)) continue;
                    
                    tempDic.Add(item.levelToSet, item.type);
                }

                return tempDic;
            }
            
            
            public void Validate(int levelAmount)
            {
                //Substracts 1 to ignore Ability.None
                int abilityCount = (int)AbilityType.Default_MAX-1;
                UpdateRarityArray(abilityCount);
                UpdateMinUnlockLevel();
                SetLevelToUnlockLimit(levelAmount);
            }
        }

        #endregion

        #region Projectile Data

        [SerializeField] private GameProjectileData projectileData;
        
        [System.Serializable]
        public class GameProjectileData
        {
            [Range(1, 50)] 
            [SerializeField] private int maxProjectileSpeed;
            [Space]
            
            [SerializeField] private SpawnMultiplier[] spawnMultiplier;
            
            [Serializable]
            public class SpawnMultiplier
            {
                [Range(1,2)]
                public float speedMultiplier;
                [Tooltip("1 is full distance")]
                [Range(0,1f)]
                public float travelRatio;
            }
            
            private void SetSpawnMultiplierLimits()
            {
                for (int i = 1; i < spawnMultiplier.Length; i++)
                {
                    if (spawnMultiplier[i].travelRatio < spawnMultiplier[i - 1].travelRatio)
                    {
                        spawnMultiplier[i].travelRatio = spawnMultiplier[i - 1].travelRatio; // clamp upward
                    }
                
                    if (spawnMultiplier[i].speedMultiplier < spawnMultiplier[i - 1].speedMultiplier)
                    {
                        spawnMultiplier[i].speedMultiplier = spawnMultiplier[i - 1].speedMultiplier; // clamp upward
                    }
                }
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

            public void Validate(int levelAmount)
            {
                UpdateArray(ref spawnMultiplier, levelAmount);
                SetSpawnMultiplierLimits();
            }
        }

        #endregion

        #region Getters

        public int LevelAmount => levelAmount;

        public int PointsMultiplier => pointsMultiplier;
        
        public GameLevelData LevelData => levelData;

        public GameAbilitySelector AbilitySelector => abilitySelector;

        public GameProjectileData ProjectileData => projectileData;

        #endregion
        
        
        private void OnValidate()
        {
            abilitySelector.Validate(levelAmount);
            levelData.ValidateByLevel(levelAmount);
            projectileData.Validate(levelAmount);
        }
    }
}