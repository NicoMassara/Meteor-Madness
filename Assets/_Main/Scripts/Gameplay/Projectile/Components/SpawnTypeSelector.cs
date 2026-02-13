using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.Common;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    public class SpawnTypeSelector
    {
        private class SpawnTypeHistory
        {
            private const int HistoryLenght = 10;
            private readonly SpawnType[] _spawnTypeHistory;
            private readonly HistoryWeightData[] _weightsHistory;
            private int _currentHistoryCount;
        
            public SpawnTypeHistory()
            {
                _spawnTypeHistory = new SpawnType[HistoryLenght];
                _weightsHistory = new HistoryWeightData[HistoryLenght];
                SendDebugGui();
            }
        
            public Dictionary<SpawnType, int> GetSpawnHistoryWeight(Dictionary<SpawnType, SpawnTypeWeight> weightsData)
            {
                var temp = new Dictionary<SpawnType, int>();

                for (int i = 1; i < (int)SpawnType.DEFAULT_MAX; i++)
                {
                    temp[(SpawnType)i] = 0;
                }
                
                var lastSpawn = SpawnType.None;
                int sameSpawnCount = 0;
                
                for (int i = _currentHistoryCount - 1; i >= 0; i--)
                {
                    var item =  _spawnTypeHistory[i];
                    if (lastSpawn == item)
                        sameSpawnCount++;
                    else
                        sameSpawnCount = 0;
        
                    var divider = i + 1;
                    
                    for (int j = 1; j < (int)SpawnType.SamePosition+1; j++)
                    {
                        var spawnType = (SpawnType)j;
                        var weight = weightsData[item].GetWeightToSpawnType(spawnType);

                        var dividedWeight = (double)weight / divider;
                        
                        if (sameSpawnCount >= 3 && spawnType == item)
                        {
                            dividedWeight *= 0.75f;
                        }
                        
                        var normalized = (int)Math.Round(dividedWeight, MidpointRounding.AwayFromZero);
        
                        temp[spawnType] += normalized;
                    }
                    
                    lastSpawn = item;
                }
        
                var keys = temp.Keys.ToList();
        
                foreach (var key in keys)
                {
                    var value = temp[key];
                    var normalized = (int)Math.Round(
                        (double)value / _currentHistoryCount,
                        MidpointRounding.AwayFromZero);
                    
                    temp[key] = normalized;
                }
                
                return temp;
            }
        
            public void AddToHistory(SpawnType spawnType, HistoryWeightData debugWeights)
            {
                if (_currentHistoryCount == 0)
                {
                    _spawnTypeHistory[0] = spawnType;
                    _weightsHistory[0] = debugWeights;
                    _currentHistoryCount++;
                }
                else
                {
                    if (_currentHistoryCount < HistoryLenght)
                        _currentHistoryCount++;
            
                    var temp1 = spawnType;
            
                    for (int i = 0; i < _currentHistoryCount; i++)
                    {
                        // ReSharper disable once SwapViaDeconstruction
                        var temp2 = _spawnTypeHistory[i];
                        _spawnTypeHistory[i] = temp1;
                        temp1 = temp2;
                    }
                    
                    var weight1 = debugWeights;
                    
                    for (int i = 0; i < _currentHistoryCount; i++)
                    {
                        // ReSharper disable once SwapViaDeconstruction
                        var weight2 = _weightsHistory[i];
                        _weightsHistory[i] = weight1;
                        weight1 = weight2;
                    }
                }
                
                SendDebugGui();
            }
        
            public void RestartData()
            {
                for (int i = 0; i < _currentHistoryCount; i++)
                {
                    _spawnTypeHistory[i] = SpawnType.None;
                }
        
                for (int i = 0; i < _currentHistoryCount; i++)
                {
                    _weightsHistory[i] = new HistoryWeightData();
                }
        
                _currentHistoryCount = 0;
                SendDebugGui();
            }
        
            private void SendDebugGui()
            {
                ProjectileDebugEvents.TriggerHistoryChanged(new HistoryDebugData
                {
                    History =  _spawnTypeHistory,
                    Amount = HistoryLenght,
                    Weights = _weightsHistory
                });
            }
        }
        
        /// <summary>
        /// Weight Data from a spawn type to another
        /// </summary>
        private class SpawnTypeWeight
        {
            private readonly Dictionary<SpawnType, int> _weightsValues;
            
            public SpawnTypeWeight(Dictionary<SpawnType, int> weightsValues)
            {
                _weightsValues = weightsValues;
            }
        
            public int GetWeightToSpawnType(SpawnType spawnType)
            {
                return _weightsValues[spawnType];
            }
        }
        
        /// <summary>
        /// Weight Value between each spawn type
        /// </summary>
        private Dictionary<SpawnType, SpawnTypeWeight> _weightsValues;
        
        private readonly SpawnTypeHistory _history;
        private bool _isFirstSpawn;
        
        public SpawnTypeSelector()
        {
            InitializeWeights();
            _history = new SpawnTypeHistory();
            _isFirstSpawn = true;
        }
        
        private void InitializeWeights()
        {
            // Weights should be from 0 to 1
            _weightsValues = new Dictionary<SpawnType, SpawnTypeWeight>
            {
                { SpawnType.Random, new SpawnTypeWeight(new()
                {
                    {SpawnType.Random, 25},
                    {SpawnType.Ascendent, 75},
                    {SpawnType.Descendent, 75},
                    {SpawnType.SamePosition, 50},
                    {SpawnType.UpAndDown, 50},
                }) },
                { SpawnType.Ascendent, new SpawnTypeWeight(new()
                {
                    {SpawnType.Random, 25},
                    {SpawnType.Ascendent, 75},
                    {SpawnType.Descendent, 100},
                    {SpawnType.SamePosition, 50},
                    {SpawnType.UpAndDown, 50},
                }) },
                { SpawnType.Descendent, new SpawnTypeWeight(new()
                {
                    {SpawnType.Random, 25},
                    {SpawnType.Ascendent, 100},
                    {SpawnType.Descendent, 75},
                    {SpawnType.SamePosition, 50},
                    {SpawnType.UpAndDown, 50},
                }) } ,
                { SpawnType.SamePosition, new SpawnTypeWeight(new()
                {
                    {SpawnType.Random, 50},
                    {SpawnType.Ascendent, 75},
                    {SpawnType.Descendent, 75},
                    {SpawnType.SamePosition, 50},
                    {SpawnType.UpAndDown, 50},
                }) },
                { SpawnType.UpAndDown, new SpawnTypeWeight(new()
                {
                    {SpawnType.Random, 50},
                    {SpawnType.Ascendent, 75},
                    {SpawnType.Descendent, 75},
                    {SpawnType.SamePosition, 50},
                    {SpawnType.UpAndDown, 50},
                }) } 
                
            };
        }
        
        private Dictionary<SpawnType, int> GetSpawnWeights(IEnumRangeData weightData)
        {
            var tempDic =  new Dictionary<SpawnType, int>();
            var historyWeight = _history.GetSpawnHistoryWeight(_weightsValues);
        
            for (int i = 0; i < weightData.WeightData.Length; i++)
            {
                var item = weightData.WeightData[i];
                var value = item.Weight;
                var historyRounded = (int)Math.Round(historyWeight[item.SpawnType] * 0.5f, MidpointRounding.AwayFromZero);
                var finalHistoryWeight = Math.Max(0, historyRounded);
        
                tempDic[item.SpawnType] = finalHistoryWeight + value;
            }
            
            return tempDic;
        }
        
        public SpawnType GetSpawnType(IEnumRangeData weightData)
        {
            var selectedSpawn = SpawnType.None;
            var debugWeights = new HistoryWeightData();
            
            if (_isFirstSpawn)
            {
                selectedSpawn = (SpawnType)RandomService.Range(1,(int)SpawnType.DEFAULT_MAX);
                _isFirstSpawn = false;
                
                debugWeights.Values = new[] { 1,1,1,1,1};
            }
            else
            {
                var finalWeights = GetSpawnWeights(weightData);
                selectedSpawn = Roulette.Run(finalWeights);
        
                debugWeights.Values = new[]
                {
                    finalWeights[SpawnType.Random],
                    finalWeights[SpawnType.Ascendent],
                    finalWeights[SpawnType.Descendent],
                    finalWeights[SpawnType.SamePosition],
                    finalWeights[SpawnType.UpAndDown]
                };
            }
            
            _history.AddToHistory(selectedSpawn, debugWeights);
            
            return selectedSpawn;
        }
        
        public void RestartData()
        {
            _isFirstSpawn = false;
            _history.RestartData();
        }
    }
}