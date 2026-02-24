using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.Common.MyRandom;
using UnityEngine;

namespace _Main.Scripts.Common.SelectorByWeight
{
    public class RandomSelector<T,TS>
        where T : Enum
        where TS : IWeightsBaseData<T>
    {
        
        #region History
        private class History<T,TS> 
            where T : Enum
            where TS : IWeightsBaseData<T>
        {
            private readonly int _length;
            private readonly T[] _spawnTypeHistory;
            private int _currentHistoryCount;

            public History(int length = 10)
            {
                _length = length;
                _spawnTypeHistory = new T[_length];
            }
            
            public Dictionary<T, int> GetHistoryWeights(IBaseWeights<T,TS> baseWeights)
            {
                var temp = ItemWeightsHelper.CreateDefaultDictionary<T>();
                var weightsData = baseWeights.GetWeights();
                var count = ItemWeightsHelper.GetCount<T>();
                
                var lastType = default(T);
                int sameItemCount = 0;
                
                for (int i = _currentHistoryCount - 1; i >= 0; i--)
                {
                    var item =  _spawnTypeHistory[i];
                    if (ItemWeightsHelper.AreItemsEqual(lastType, item))
                        sameItemCount++;
                    else
                        sameItemCount = 0;
        
                    var divider = i + 1;
                    
                    for (int j = 1; j < count-1; j++)
                    {
                        var itemType = ItemWeightsHelper.FromInt<T>(j);
                        var weight = weightsData[item].GetWeightToSpawnType(itemType);

                        var dividedWeight = (double)weight / divider;
                        
                        if (sameItemCount >= 3 && ItemWeightsHelper.AreItemsEqual(lastType, item))
                        {
                            dividedWeight *= 0.75f;
                        }
                        
                        var normalized = (int)Math.Round(dividedWeight, MidpointRounding.AwayFromZero);
        
                        temp[itemType] += normalized;
                    }
                    
                    lastType = item;
                }
        
                var keys = temp.Keys.ToList();
        
                foreach (var key in keys)
                {
                    if (temp.TryGetValue(key, out var value))
                    {
                        var normalized = (int)Math.Round(
                            (double)value / _currentHistoryCount,
                            MidpointRounding.AwayFromZero);
                        
                        temp[key] = normalized;
                    }
                }
                
                return temp;
            }
            
            public void AddToHistory(T spawnType)
            {
                if (_currentHistoryCount == 0)
                {
                    _spawnTypeHistory[0] = spawnType;
                    _currentHistoryCount++;
                }
                else
                {
                    if (_currentHistoryCount < _length)
                        _currentHistoryCount++;
            
                    var temp1 = spawnType;
            
                    for (int i = 0; i < _currentHistoryCount; i++)
                    {
                        // ReSharper disable once SwapViaDeconstruction
                        var temp2 = _spawnTypeHistory[i];
                        _spawnTypeHistory[i] = temp1;
                        temp1 = temp2;
                    }
                }
            }
            
            public void RestartData()
            {
                for (int i = 0; i < _currentHistoryCount; i++)
                {
                    _spawnTypeHistory[i] = default(T);
                }
        
                _currentHistoryCount = 0;
            }
        }
        
        #endregion
        
        private readonly History<T,TS> _history;
        private readonly bool _doesHasHistory;
        private bool _isFirstSpawn;

        public RandomSelector()
        {
            _doesHasHistory = false;
            _isFirstSpawn = true;
        }

        public RandomSelector(int historyLength)
        {
            _isFirstSpawn = true;
            _doesHasHistory = true;
            _history = new History<T,TS>(historyLength);
        }
        
        private Dictionary<T, int> GetSpawnWeights(Dictionary<T, int> weights, IBaseWeights<T,TS> baseWeights)
        {
            var tempDic = ItemWeightsHelper.CreateDefaultDictionary<T>();
            var historyWeight = _history.GetHistoryWeights(baseWeights);
            var count = ItemWeightsHelper.GetCount<T>();
            
            for (int i = 0; i < count; i++)
            {
                var currType = ItemWeightsHelper.FromInt<T>(i+1);
                var value = 0;

                if (weights.ContainsKey(currType) == false)
                {
                    weights.Add(currType, 0);
                }
                else
                {
                    value = weights[currType];
                }
                
                if (value > 0)
                {
                    var historyRounded = (int)Math.Round(historyWeight[currType] * 0.5f, MidpointRounding.AwayFromZero);
                    var finalHistoryWeight = Math.Max(0, historyRounded);
                
                    tempDic[currType] = finalHistoryWeight + value;
                }
                else
                {
                    tempDic[currType] = 0;
                }
            }
            
            
            return tempDic;
        }

        public T GetRandomItem(Dictionary<T, int> currentData, IBaseWeights<T,TS> baseWeights)
        {
            var selectedItem = default(T);

            if (currentData.Count == 0)
            {
                throw new Exception("Weight Data is empty");
            }


            Dictionary<T, int> finalWeights = null;
            
            if (_isFirstSpawn)
            {
                finalWeights = currentData;
                _isFirstSpawn = false;
            }
            else
            {
                if (_doesHasHistory)
                {
                    finalWeights = GetSpawnWeights(currentData, baseWeights);
                }
                else
                {
                    finalWeights = currentData;
                }
            }
            
            selectedItem = Roulette.Run(finalWeights);
            
            if(_doesHasHistory)
                _history.AddToHistory(selectedItem);
            
            return selectedItem;
        }
        
        public void RestartData()
        {
            _isFirstSpawn = false;
            _history.RestartData();
        }

        public void ClearHistory()
        {
            if(_doesHasHistory)
                _history.RestartData();
        }
    }
}