using System;
using System.Collections.Generic;
using _Main.Scripts.Common.SelectorByWeight;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Abilities
{
    public interface IAbilityView : IObserver
    {
        public event Action OnAbilityFinished;
    }
    
    public interface IAbilitySelector
    {
        public IAbilityBaseWeights GetWeights();
        public Dictionary<AbilityType, int> GetBaseWeights();
    }
    
    [System.Serializable]
    public class WeightsPair : WeightsPairBase<AbilityType> { }
    [System.Serializable]
    public class WeightsData : WeightsBaseData<AbilityType, WeightsPair>, IAbilityWeightsData { }
    public interface IAbilityWeightsData : IWeightsBaseData<AbilityType> { }
    public interface IAbilityBaseWeights : IBaseWeights<AbilityType, IAbilityWeightsData> { }

    [System.Serializable]
    public class AbilityBaseWeights : IAbilityBaseWeights
    {
        public WeightsData[] weights;
        
        private Dictionary<AbilityType, IAbilityWeightsData> _cache;

        public void BuildCache()
        {
            _cache = new Dictionary<AbilityType, IAbilityWeightsData>();
            foreach (var w in weights)
            {
                _cache[w.itemType] = w;
            }
        }

        public Dictionary<AbilityType, IAbilityWeightsData> GetWeights()
        {
            if(_cache == null)
                BuildCache();
            
            return _cache;
        }

        public void TryCreateDefaultArray()
        {
            if (weights == null || weights.Length != (int)AbilityType.Default_MAX - 1)
            {
                var count = (int)AbilityType.Default_MAX - 1;
                weights = new WeightsData[count];
            
                for (int i = 0; i < count; i++)
                {
                    var item = new WeightsData
                    {
                        itemType = (AbilityType)i + 1
                    };
                    
                    item.CreateDefaultData();

                    weights[i] = item;
                }
            }
        }
    }
}