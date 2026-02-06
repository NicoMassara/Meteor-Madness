using System;
using System.Collections.Generic;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.Tools;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileSlotSelector
    {
        private class SpawnTypeSelector
        {
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

                public float GetWeightToSpawnType(SpawnType spawnType)
                {
                    return _weightsValues[spawnType];
                }
            }

            /// <summary>
            /// Weight Value between each spawn type
            /// </summary>
            private Dictionary<SpawnType, SpawnTypeWeight> _weightsValues;
            
            private const int HistoryLenght = 10;
            private readonly SpawnType[] _spawnTypeHistory;
            private int _currentHistoryCount;
            
            private SpawnType _lastSpawnType;
            
            public SpawnTypeSelector()
            {
                InitializeWeights();
                _spawnTypeHistory = new SpawnType[HistoryLenght];
            }

            private void InitializeWeights()
            {
                // Weights should be from 0 to 1
                _weightsValues = new Dictionary<SpawnType, SpawnTypeWeight>
                {
                    { SpawnType.Random, new SpawnTypeWeight(new()
                    {
                        {SpawnType.Random, 25},
                        {SpawnType.Ascendent, 100},
                        {SpawnType.Descendent, 100},
                        {SpawnType.SamePosition, 100},
                    }) },
                    { SpawnType.Ascendent, new SpawnTypeWeight(new()
                    {
                        {SpawnType.Random, 25},
                        {SpawnType.Ascendent, 50},
                        {SpawnType.Descendent, 75},
                        {SpawnType.SamePosition, 50},
                    }) },
                    { SpawnType.Descendent, new SpawnTypeWeight(new()
                    {
                        {SpawnType.Random, 25},
                        {SpawnType.Ascendent, 75},
                        {SpawnType.Descendent, 50},
                        {SpawnType.SamePosition, 50},
                    }) } ,
                    { SpawnType.SamePosition, new SpawnTypeWeight(new()
                    {
                        {SpawnType.Random, 50},
                        {SpawnType.Ascendent, 50},
                        {SpawnType.Descendent, 75},
                        {SpawnType.SamePosition, 25},
                    }) } 
                };
            }

            private void AddToHistory(SpawnType spawnType)
            {
                if (_currentHistoryCount == 0)
                {
                    _spawnTypeHistory[0] = spawnType;
                    _currentHistoryCount++;
                    return;
                }
                
                var temp1 = spawnType;
                
                for (int i = 0; i < _currentHistoryCount; i++)
                {
                    // ReSharper disable once SwapViaDeconstruction
                    var temp2 = _spawnTypeHistory[i];
                    _spawnTypeHistory[i] = temp1;
                    temp1 = temp2;
                }
                
                if (_currentHistoryCount < HistoryLenght)
                    _currentHistoryCount++;
            }

            public SpawnType GetSpawnType(IEnumRangeData weightData)
            {
                
                
                return SpawnType.Ascendent;
            }
        }

        private const int SlotsAmount = GameParameters.GameplayValues.AngleSlots;
        private readonly IProjectileSpawnData _data;
        private readonly SpawnTypeSelector _spawnTypeSelector;
        private SlotData[] _currentBatch;
        private int _currentBatchIndex;
        private int _currentLevel;
        private int _lastSelectedSlot = -1;
        
        public ProjectileSlotSelector(IProjectileSpawnData data)
        {
            _data = data;
            _lastSelectedSlot = -1;
            
            InitializeData();
        }

        public void InitializeData()
        {
            _currentLevel = 0;
            _lastSelectedSlot = -1;
        }

        public void Restart()
        {
            _currentLevel = 0;
            _lastSelectedSlot = -1;
            InitializeData();
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;
        }

        public int CreateBatchData()
        {
            var spawnData = GetCurrentSpawnData();

            var selectedSlot = 0;
            
            if (_lastSelectedSlot == -1)
            {
                selectedSlot = RandomService.Range(0, SlotsAmount);
            }
            else if (_lastSelectedSlot > -1)
            {
                var slotRange = GetRandomDirection() * spawnData.NextBatchSlotRange.RandomRange;
                selectedSlot = _lastSelectedSlot + slotRange;
            }
            
            var spawnType = _spawnTypeSelector.GetSpawnType(spawnData.SpawnTypeRange);
            var selectedAmount = spawnData.ProjectileAmountRange.RandomRange;
            var innerBatchDistance = spawnData.InnerBatchDistanceRange.RandomRange;

            _currentBatch = new SlotData[selectedAmount];

            if (selectedAmount == 1)
            {
                _currentBatch[0] = new SlotData
                {
                    Slot = selectedSlot,
                    DistanceRatio = spawnData.NextBatchDistanceRange.RandomRange
                };
                
                _lastSelectedSlot = _currentBatch[0].Slot;
                _currentBatchIndex = selectedAmount - 1;
                return selectedAmount;
            }
            
            var currentOffset = 0;
            var nextBatchDistance =  spawnData.NextBatchDistanceRange.RandomRange;

            if (spawnType is SpawnType.Ascendent or SpawnType.Descendent)
            {
                currentOffset = spawnData.SlotRange.Range.x;
            }
            else
            {
                currentOffset = spawnData.SlotRange.RandomRange;;
            }

            var currentSlot = selectedSlot;
            
            for (var i = selectedAmount - 1; i >= 0; i--)
            {
                if (i != selectedAmount - 1)
                {
                    switch (spawnType)
                    {
                        case SpawnType.Random:
                            currentSlot += currentOffset * GetRandomDirection();
                            break;

                        case SpawnType.Ascendent:
                            currentSlot += currentOffset;
                            break;

                        case SpawnType.Descendent:
                            currentSlot -= currentOffset;
                            break;
                    }

                    currentSlot = (int)Mathf.Repeat(currentSlot, SlotsAmount);
                }

                _currentBatch[i] = new SlotData
                {
                    Slot = currentSlot,
                    DistanceRatio = (i == 0) ? 
                        nextBatchDistance : 
                        innerBatchDistance
                };
            }

            _lastSelectedSlot = _currentBatch[0].Slot;
            _currentBatchIndex = selectedAmount - 1;
            

            
            ProjectileDebugEvents.TriggerBatchCreated(new BatchDebugData
            {
                Level = _currentLevel,
                Amount = selectedAmount,
                StartSlot = selectedSlot,
                Offset =  currentOffset,
                InnerDist = innerBatchDistance,
                NextDist = nextBatchDistance,
                LastSlot = _lastSelectedSlot,
                SpawnType  = spawnType
            });
            
            return selectedAmount;
        }
        
        public SlotData GetNextSlotData()
        {
            var value = _currentBatch[_currentBatchIndex];
            _currentBatchIndex--;
            
            return value;
        }

        private IBatchSpawnData GetCurrentSpawnData() => _data.GetDataByIndex(_currentLevel);

        private int GetRandomDirection() => RandomService.Value() > 0.5f ? -1 : 1;
    }
}