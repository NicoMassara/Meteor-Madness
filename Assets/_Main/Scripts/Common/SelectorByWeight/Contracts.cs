using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Main.Scripts.Common.SelectorByWeight
{
    #region Interfaces

    public interface IBaseWeights<T,TS>
        where T : Enum
        where TS : IWeightsBaseData<T>
    {
        public Dictionary<T, TS> GetWeights();
    }

    public interface IWeightsBaseData<T>
        where T : Enum
    {
        public Dictionary<T, int> GetWeights();
        public int GetWeightToSpawnType(T typeSelected);
    }

    #endregion
    
    #region Classes

    [System.Serializable]
    public class WeightsPairBase<T>
        where T : Enum
    {
        public T key;
        [Min(0)] 
        public int weight = 50;
    }
    
    [System.Serializable]
    public class WeightsBaseData<T,TS> : IWeightsBaseData<T>
        where T : Enum
        where TS : WeightsPairBase<T>, new()
    {
        public T itemType;
        public TS[] weights;
        
        private Dictionary<T, int> _cache;

        public void BuildCache()
        {
            _cache = ItemWeightsHelper.CreateDefaultDictionary<T>();
            foreach (var w in weights)
            {
                _cache[w.key] = w.weight;
            }
        }

        public Dictionary<T, int> GetWeights()
        {
            if (_cache == null)
                BuildCache();

            return _cache;
        }

        public int GetWeightToSpawnType(T typeSelected)
        {
            if (_cache == null)
                BuildCache();

            return _cache.TryGetValue(typeSelected, out var value) ? value : 0;
        }

        public void CreateDefaultData()
        {
            var count = ItemWeightsHelper.GetCount<T>() - 2;
            weights = new TS[count];

            for (int i = 0; i < count; i++)
            {
                var item = new TS();
                var currItem = ItemWeightsHelper.FromInt<T>(i + 1);
                item.key = currItem;
                weights[i] = item;
            }
        }
    }
    
    #endregion

    #region Helpers
    
    internal class ItemWeightsHelper
    {
        public static Dictionary<T, int> CreateDefaultDictionary<T>()
            where T : Enum
        {
            var dict = new Dictionary<T, int>();
            var values = (T[])Enum.GetValues(typeof(T));

            for (int i = 0; i < values.Length - 2; i++)
            {
                dict[values[i+1]] = 0;
            }
            
            return dict;
        }
        
        public static bool AreItemsEqual<T>(T a, T b) where T : Enum => a.Equals(b);
        public static int GetCount<T>() where T : Enum => Enum.GetValues(typeof(T)).Length;
        public static T FromInt<T>(int value) where T : Enum => (T)Enum.ToObject(typeof(T), value);
    }
    
    
    #endregion
}