using System.Collections.Generic;
using MeteorMadness.Contracts;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Abilities;
using UnityEngine;

namespace MeteorMadness.Gameplay.Abilities.So
{
    [CreateAssetMenu(fileName = "SO_AbilitySelectorData_Default", menuName = "Scriptable Objects/Ability/Selector Data", order = 0)]
    public class AbilitySelectorDataSo : ScriptableObject, IAbilitySelector
    {
        [System.Serializable]
        private struct SingleWeight
        {
            public AbilityType abilityType;
            [Range(0,200)]
            public int weight;
        }

        [System.Serializable]
        private struct BaseWeights
        {
            [SerializeField] private SingleWeight[] items;

            public void CreateArray()
            {
                if (items == null || items.Length != (int)AbilityType.Default_MAX - 1)
                {
                    var count = (int)AbilityType.Default_MAX - 1;
                    items = new SingleWeight[count];
            
                    for (int i = 0; i < count; i++)
                    {
                        var item = new SingleWeight
                        {
                            abilityType = (AbilityType)i + 1
                        };
                        
                        items[i] = item;
                    }
                }
            }
            
            public Dictionary<AbilityType, int> GetWeights()
            {
                var temp  = new Dictionary<AbilityType, int>();

                foreach (var item in items)
                {
                    if(temp.ContainsKey(item.abilityType)) continue;
                
                    temp[item.abilityType] = item.weight;
                }
                
                return temp;
            }
            
        }
        
        [SerializeField] private BaseWeights baseWeights;

        [SerializeField] private AbilityBaseWeights weights;
        
        public IAbilityBaseWeights GetWeights() => weights;
        
        public Dictionary<AbilityType, int> GetBaseWeights() => baseWeights.GetWeights();

        private void OnValidate()
        {
            SetArray();
        }
        
        private void OnEnable()
        {
            SetArray();
        }

        private void SetArray()
        {
            baseWeights.CreateArray();
            weights?.TryCreateDefaultArray();
        }
    }
}