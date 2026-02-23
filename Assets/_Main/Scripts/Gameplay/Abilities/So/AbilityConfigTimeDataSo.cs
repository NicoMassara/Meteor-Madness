using System;
using _Main.Scripts.Common.MyRandom;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Managers.GameConfig.Game;
using Unity.Collections;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities.So
{
    [CreateAssetMenu(fileName = "SO_AbilityConfigTimeData_Name", menuName = "Scriptable Objects/Ability/Time Config Data", order = 0)]
    public class AbilityConfigTimeDataSo : ScriptableObject, IAbilityTimeConfigData
    {
        [System.Serializable]
        internal struct AmountRangeData
        {
            [Min(1)] 
            [SerializeField] private int minAmount;
            [Min(1)] 
            [SerializeField] private int maxAmount;
        
            public Vector2Int GetRange() => new Vector2Int(minAmount, maxAmount);
            public int GetRandomRange() => RandomService.Range(minAmount, maxAmount);
        }

        [SerializeField] private AmountRangeData batchAmount;
        
        [SerializeField] private AbilityData[] abilities;
        
        public IAbilityTimeData GetAbilityTimeData(AbilityType abilityType)
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                if (abilities[i].type == abilityType)
                {
                    return abilities[i].timeData;
                }
            }
            
            return null;
        }

        public int GetBatchAmount() => batchAmount.GetRandomRange();

        [Serializable]
        private class AbilityData
        {
            [ReadOnly]
            public AbilityType type;
            public AbilityTimeDataSo timeData;
        }
        
        private void SetAbilitiesArray(int abilityCount)
        {
            for (int i = 0; i < abilityCount; i++)
            {
                abilities[i] = new AbilityData
                {
                    type = (AbilityType)i+1,
                };
            }
        }
        
        private void OnValidate()
        {
            int abilityCount = (int)AbilityType.Default_MAX-1;
            GameConfigUtilities.UpdateArray(ref abilities, abilityCount, SetAbilitiesArray);
        }
    }
}