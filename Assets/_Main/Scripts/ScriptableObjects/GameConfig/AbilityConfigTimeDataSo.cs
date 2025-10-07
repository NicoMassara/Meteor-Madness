using System;
using _Main.Scripts.InspectorTools;
using _Main.Scripts.Interfaces;
using _Main.Scripts.ScriptableObjects.AbilityTime;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects.GameConfig
{
    [CreateAssetMenu(fileName = "SO_AbilityConfigTimeData_Name", menuName = "Scriptable Objects/Game Config/Ability/Time Config Data", order = 0)]
    public class AbilityConfigTimeDataSo : ScriptableObject, IAbilityTimeConfigData
    {
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