using System;
using System.Collections.Generic;
using System.Linq;
using _Main.Scripts.Common;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileSlotSelector
    {
        private struct BatchSpawnData
        {
            public float SpeedMultiplier;
            public int Amount;
            public int SlotRange;
            public float InnerDistance;
            public float NextDistance;
            public float Delay;
            public int NextSlotRange;
            public SpawnType SpawnType;

            public int MinSlotRange;
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

            _spawnTypeSelector = new SpawnTypeSelector();
            
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
            _spawnTypeSelector.RestartData();
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
                var slotRange = GetRandomDirection() * spawnData.NextSlotRange;
                selectedSlot = _lastSelectedSlot + slotRange;
            }
            
            var selectedAmount = spawnData.Amount;
            var innerBatchDistance = spawnData.InnerDistance;

            _currentBatch = new SlotData[selectedAmount];
            var currentOffset = 0;
            var nextBatchDistance =  spawnData.NextDistance;
            
            if (selectedAmount == 1)
            {
                _currentBatch[0] = new SlotData
                {
                    Slot = selectedSlot,
                    DistanceRatio = nextBatchDistance
                };
                
                _lastSelectedSlot = _currentBatch[0].Slot;
                _currentBatchIndex = selectedAmount - 1;
            }
            else
            {
                if (spawnData.SpawnType is SpawnType.Ascendent or SpawnType.Descendent)
                {
                    currentOffset = spawnData.MinSlotRange;
                }
                else
                {
                    currentOffset = spawnData.SlotRange;
                }

                var currentSlot = selectedSlot;
            
                for (var i = selectedAmount - 1; i >= 0; i--)
                {
                    if (i != selectedAmount - 1)
                    {
                        switch (spawnData.SpawnType)
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
                            innerBatchDistance,
                        MovementSpeed = _data.ProjectileSpeed * spawnData.SpeedMultiplier
                    };
                }

                _lastSelectedSlot = _currentBatch[0].Slot;
                _currentBatchIndex = selectedAmount - 1;
            }
            
            ProjectileDebugEvents.TriggerBatchCreated(new BatchDebugData
            {
                Level = _currentLevel,
                Amount = selectedAmount,
                StartSlot = selectedSlot,
                Offset =  currentOffset,
                InnerDist = innerBatchDistance,
                NextDist = nextBatchDistance,
                LastSlot = _lastSelectedSlot,
                SpawnType  = spawnData.SpawnType
            });
            
            return selectedAmount;
        }
        
        public SlotData GetNextSlotData()
        {
            var value = _currentBatch[_currentBatchIndex];
            _currentBatchIndex--;
            
            return value;
        }

        private BatchSpawnData GetCurrentSpawnData()
        {
            return _currentLevel < GameParameters.GameplayValues.SpawnLevelAmount ? 
                CreateBatchSpawnData(_currentLevel) : 
                CreateBatchSpawnData(GameParameters.GameplayValues.SpawnLevelAmount-1);
        }

        private BatchSpawnData CreateBatchSpawnData(int index)
        {
            var temp = _data.GetDataByIndex(index);
            
            return new BatchSpawnData
            {
                SpeedMultiplier = temp.SpeedMultiplierRange.RandomRange,
                Amount = temp.ProjectileAmountRange.RandomRange,
                SlotRange = temp.SlotRange.RandomRange,
                NextDistance = temp.NextBatchDistanceRange.RandomRange,
                Delay = temp.NextBatchDelayRange.RandomRange,
                NextSlotRange = temp.NextBatchSlotRange.RandomRange,
                SpawnType = _spawnTypeSelector.GetSpawnType(temp.SpawnTypeRange),
                MinSlotRange = temp.SlotRange.Range.x
            };
        }

        private int GetRandomDirection() => RandomService.Value() > 0.5f ? -1 : 1;
    }
}