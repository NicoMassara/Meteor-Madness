using System;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    public abstract class ProjectileBatchBase
    {
        protected struct BatchSpawnData
        {
            public float SpeedMultiplier;
            public int Amount;
            public int SlotRange;
            public float InnerDistance;
            public float NextDistance;
            public float Delay;
            public int NextSlotRange;
            public SpawnType SpawnType;
            public bool HasAbility;

            public int MinSlotRange;
            public ISlotRangeData SlotRangeData;
        }

        public struct ProjectileBatchData
        {
            public int SlotAmount;
            public float ProjectileValue;
        }

        private readonly ProjectileBatchData _batchData;
        private readonly SpawnTypeSelector _spawnTypeSelector;
        private SlotData[] _currentBatch;
        private int _lastSelectedSlot;
        private int _currentBatchIndex;

        protected ProjectileBatchData GetBatchData() => _batchData;

        protected event Action<BatchDebugData> OnDebugBatchCreated;
        

        public ProjectileBatchBase(ProjectileBatchData data)
        {
            _batchData = data;
            _lastSelectedSlot = -1;
            _spawnTypeSelector = new SpawnTypeSelector();
        }

        public virtual void Restart()
        {
            Debug.Log("Here");
            _lastSelectedSlot = -1;
        }
        
        public int CreateBatchData()
        {
            var spawnData = GetSpawnData();
            var selectedSlot = 0;
            
            if (_lastSelectedSlot == -1)
            {
                selectedSlot = RandomService.Range(0, GetBatchData().SlotAmount);
            }
            else if (_lastSelectedSlot > -1)
            {
                var slotRange = GetRandomDirection() * spawnData.NextSlotRange;
                selectedSlot = _lastSelectedSlot + slotRange;
            }
            
            var selectedAmount = spawnData.Amount;
            var innerBatchDistance = spawnData.InnerDistance;

            _currentBatch = new SlotData[selectedAmount];
            var nextBatchDistance =  spawnData.NextDistance;
            var currentOffset = 0;
            
            if (selectedAmount == 1)
            {
                CreateSingleProjectileBatch(ref _currentBatch, new SingleProjectileBatchData
                {
                    SelectedSlot = selectedSlot,
                    NextBatchDistance = nextBatchDistance,
                    SpeedMultiplier = spawnData.SpeedMultiplier,
                });
            }
            else
            {
                switch (spawnData.SpawnType)
                {
                    case SpawnType.Random:
                        
                        CreateRandomBatch(ref _currentBatch, new RandomBatchData
                        {
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            NextBatchDistance = spawnData.NextDistance,
                            InnerBatchDistance = spawnData.InnerDistance,
                            SpeedMultiplier = spawnData.SpeedMultiplier,
                            SlotRangeData = spawnData.SlotRangeData
                        });
                        break;
                    case SpawnType.Ascendent or SpawnType.Descendent:
                            
                        CreateAscendentBatchData(ref _currentBatch, new AscendentBatchData
                        {
                            SlotOffset =  spawnData.MinSlotRange,
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            IsAscendent = spawnData.SpawnType == SpawnType.Ascendent,
                            NextBatchDistance = spawnData.NextDistance,
                            InnerBatchDistance = spawnData.InnerDistance,
                            SpeedMultiplier = spawnData.SpeedMultiplier,
                        });
                        break;
                    case SpawnType.SamePosition:
                        
                        CreateSamePositionBatch(ref _currentBatch, new SamePositionBatchData
                        {
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            NextBatchDistance = spawnData.NextDistance,
                            SpeedMultiplier = spawnData.SpeedMultiplier,
                        });
                        break;
                    case SpawnType.UpAndDown:
                        
                        CreateUpAndDownBatchBatch(ref _currentBatch, new UpAndDownBatchData
                        {
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            NextBatchDistance = spawnData.NextDistance,
                            InnerBatchDistance = spawnData.InnerDistance,
                            SpeedMultiplier = spawnData.SpeedMultiplier,
                            SlotRangeData = spawnData.SlotRangeData
                        });
                        break;
                }
                
                _lastSelectedSlot = _currentBatch[0].Slot;
                _currentBatchIndex = selectedAmount - 1;
            }
            
            OnDebugBatchCreated?.Invoke(new BatchDebugData
            {
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

        private void CreateAscendentBatchData(ref SlotData[] batchData, AscendentBatchData data)
        {
            var isAbility = false;
            var hasSpawnedAbility = false;
            var currentSlot = data.SelectedSlot;

            for (int i = 0; i < data.SelectedAmount - 1; i++)
            {
                // Set if current projectile is ability
                if (data.HasAbility && hasSpawnedAbility == false)
                {
                    if (i == data.SelectedAmount - 1)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                    else
                    {
                        var value = RandomService.Value();
                        if (value >= 0.45f)
                        {
                            isAbility = true;
                            hasSpawnedAbility = true;
                        }
                    }
                }
                
                //Calculate direction
                if (data.IsAscendent)
                {
                    currentSlot += data.SlotOffset;
                }
                else
                {
                    currentSlot -= data.SlotOffset;
                }
                
                //Clamps Slot
                var slotAmount = GetBatchData().SlotAmount;
                currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = currentSlot,
                    DistanceRatio = (i == 0) ? data.NextBatchDistance : data.InnerBatchDistance,
                    MovementSpeed = data.SpeedMultiplier,
                    IsAbility = isAbility
                };
                
                batchData[i] = OverrideSlotData(slotData, i + 1, data.SelectedAmount);
            }
        }
        
        private struct AscendentBatchData
        {
            public int SlotOffset;
            public int SelectedSlot;
            public int SelectedAmount;
            public bool HasAbility;
            public bool IsAscendent;
            public float NextBatchDistance;
            public float InnerBatchDistance;
            public float SpeedMultiplier;
        }

        private void CreateSamePositionBatch(ref SlotData[] batchData, SamePositionBatchData data)
        {
            var isAbility = false;
            var hasSpawnedAbility = false;

            for (int i = 0; i < data.SelectedAmount - 1; i++)
            {
                // Set if current projectile is ability
                if (data.HasAbility && hasSpawnedAbility == false)
                {
                    if (i == data.SelectedAmount - 1)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                    else
                    {
                        var value = RandomService.Value();
                        if (value >= 0.45f)
                        {
                            isAbility = true;
                            hasSpawnedAbility = true;
                        }
                    }
                }
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = data.SelectedSlot,
                    DistanceRatio = (i == 0) ? data.NextBatchDistance : 0,
                    MovementSpeed = data.SpeedMultiplier,
                    IsAbility = isAbility
                };
                
                batchData[i] = OverrideSlotData(slotData, i + 1, data.SelectedAmount);
            }
        }
        
        private struct SamePositionBatchData
        {
            public int SelectedSlot;
            public int SelectedAmount;
            public bool HasAbility;
            public float NextBatchDistance;
            public float SpeedMultiplier;
        }
        
        private void CreateRandomBatch(ref SlotData[] batchData, RandomBatchData data)
        {
            var isAbility = false;
            var hasSpawnedAbility = false;
            var currentSlot = data.SelectedSlot;

            for (int i = 0; i < data.SelectedAmount - 1; i++)
            {
                // Set if current projectile is ability
                if (data.HasAbility && hasSpawnedAbility == false)
                {
                    if (i == data.SelectedAmount - 1)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                    else
                    {
                        var value = RandomService.Value();
                        if (value >= 0.45f)
                        {
                            isAbility = true;
                            hasSpawnedAbility = true;
                        }
                    }
                }

                currentSlot += data.SlotRangeData.RandomRange * GetRandomDirection();
                
                //Clamps Slot
                var slotAmount = GetBatchData().SlotAmount;
                currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = currentSlot,
                    DistanceRatio = (i == 0) ? data.NextBatchDistance : data.InnerBatchDistance,
                    MovementSpeed = data.SpeedMultiplier,
                    IsAbility = isAbility
                };
                
                batchData[i] = OverrideSlotData(slotData, i + 1, data.SelectedAmount);
            }
        }
        
        private struct RandomBatchData
        {
            public int SelectedSlot;
            public int SelectedAmount;
            public bool HasAbility;
            public float NextBatchDistance;
            public float InnerBatchDistance;
            public float SpeedMultiplier;
            public ISlotRangeData SlotRangeData;
        }
        
        private void CreateUpAndDownBatchBatch(ref SlotData[] batchData, UpAndDownBatchData data)
        {
            var isAbility = false;
            var hasSpawnedAbility = false;
            var direction = GetRandomDirection();
            var currentSlot =  data.SelectedSlot;
            
            for (int i = data.SelectedAmount - 1; i >= 0; i--)
            {
                // Set if current projectile is ability
                if (data.HasAbility && hasSpawnedAbility == false)
                {
                    if (i == 0)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                    else
                    {
                        var value = RandomService.Value();
                        if (value >= 0.45f)
                        {
                            isAbility = true;
                            hasSpawnedAbility = true;
                        }
                    }
                }

                if (i < data.SelectedAmount - 1)
                {
                    currentSlot = data.SelectedSlot + (data.SlotRangeData.RandomRange * direction);
                    direction *= -1;
                }
                
                //Clamps Slot
                var slotAmount = GetBatchData().SlotAmount;
                currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = currentSlot,
                    DistanceRatio = (i == 0) ? data.NextBatchDistance : data.InnerBatchDistance,
                    MovementSpeed = data.SpeedMultiplier,
                    IsAbility = isAbility
                };
                
                batchData[i] = OverrideSlotData(slotData, i + 1, data.SelectedAmount);
            }
            
        }
        
        private struct UpAndDownBatchData
        {
            public int SelectedSlot;
            public int SelectedAmount;
            public bool HasAbility;
            public float NextBatchDistance;
            public float InnerBatchDistance;
            public float SpeedMultiplier;
            public ISlotRangeData SlotRangeData;
        }
        

        private void CreateSingleProjectileBatch(ref SlotData[] batchData, SingleProjectileBatchData data)
        {
            var slotData = new SlotData
            {
                Slot = data.SelectedSlot,
                DistanceRatio = data.NextBatchDistance,
                MovementSpeed = data.SpeedMultiplier,
            };
                    
            batchData[0] = OverrideSlotData(slotData, 1, 1);
        }

        private struct SingleProjectileBatchData
        {
            public int SelectedSlot;
            public float NextBatchDistance;
            public float SpeedMultiplier;
        }

        protected SpawnType GetSpawnType(IEnumRangeData baseWeights) => _spawnTypeSelector.GetSpawnType(baseWeights);

        protected abstract SlotData OverrideSlotData(SlotData data,int index, int batchAmount);

        private int GetRandomDirection() => RandomService.Value() > 0.5f ? -1 : 1;
        
        protected abstract BatchSpawnData GetSpawnData();
    }
}