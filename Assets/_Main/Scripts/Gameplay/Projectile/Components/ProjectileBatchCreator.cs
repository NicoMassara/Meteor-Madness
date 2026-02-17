using System;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    public class ProjectileBatchCreator
    {
        private readonly int _angleSlots;
        private SlotData[] _currentBatch;
        private int _lastSelectedSlot;
        private int _currentBatchIndex;

        protected event Action<BatchDebugData> OnDebugBatchCreated;
        
        public ProjectileBatchCreator(int angleSlots)
        {
            _angleSlots = angleSlots;
            _lastSelectedSlot = -1;
        }

        public virtual void RestartValues()
        {
            _lastSelectedSlot = -1;
        }
        
        public int CreateBatchData(BatchSpawnData spawnData, Func<int,int,float> getProjectileValue)
        {
            var selectedSlot = 0;
            
            if (_lastSelectedSlot == -1)
            {
                selectedSlot = RandomService.Range(0, _angleSlots);
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
                    MovementSpeed = spawnData.MovementSpeed,
                }, getProjectileValue);
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
                            MovementSpeed = spawnData.MovementSpeed,
                            SlotRangeData = spawnData.SlotRangeData
                        }, getProjectileValue);
                        break;
                    
                    case SpawnType.Ascendent or SpawnType.Descendent:
                        CreateAscendentBatchData(ref _currentBatch, new AscendentBatchData
                        {
                            SlotOffset =  spawnData.SlotRange,
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            IsAscendent = spawnData.SpawnType == SpawnType.Ascendent,
                            NextBatchDistance = spawnData.NextDistance,
                            InnerBatchDistance = spawnData.InnerDistance,
                            MovementSpeed = spawnData.MovementSpeed,
                        }, getProjectileValue);
                        break;
                    
                    case SpawnType.SamePosition:
                        CreateSamePositionBatch(ref _currentBatch, new SamePositionBatchData
                        {
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            NextBatchDistance = spawnData.NextDistance,
                            MovementSpeed = spawnData.MovementSpeed,
                        }, getProjectileValue);
                        break;
                    
                    case SpawnType.UpAndDown:
                        CreateUpAndDownBatchBatch(ref _currentBatch, new UpAndDownBatchData
                        {
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            NextBatchDistance = spawnData.NextDistance,
                            InnerBatchDistance = spawnData.InnerDistance,
                            MovementSpeed = spawnData.MovementSpeed,
                            SlotRangeData = spawnData.SlotRangeData
                        }, getProjectileValue);
                        break;
                }
            }
            
            _lastSelectedSlot = _currentBatch[0].Slot;
            _currentBatchIndex = selectedAmount - 1;
            
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

        #region Ascendent/Descendent
        
        private void CreateAscendentBatchData(ref SlotData[] batchData, AscendentBatchData data, Func<int,int,float> getProjectileValue)
        {
            var hasSpawnedAbility = false;
            var currentSlot = data.SelectedSlot;
            var slotAmount = _angleSlots;

            for (int i = 0; i < data.SelectedAmount; i++)
            {
                var isAbility = false;
                // Set if current projectile is ability
                if (data.HasAbility && hasSpawnedAbility == false)
                {
                    if (i == data.SelectedAmount - 1)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                    else if (RandomService.Value() >= 0.45f)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
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
                currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = currentSlot,
                    DistanceRatio = (i == 0) ? data.NextBatchDistance : data.InnerBatchDistance,
                    MovementSpeed = data.MovementSpeed,
                    FinalValue = getProjectileValue.Invoke(i, data.SelectedAmount),
                    IsAbility = isAbility
                };

                batchData[i] = slotData;
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
            public float MovementSpeed;
        }
        
        
        #endregion
        
        #region SamePosition

        private void CreateSamePositionBatch(ref SlotData[] batchData, SamePositionBatchData data, Func<int,int,float> getProjectileValue)
        {
            var hasSpawnedAbility = false;

            for (int i = 0; i < data.SelectedAmount; i++)
            {
                var isAbility = false;
                // Set if current projectile is ability
                if (data.HasAbility && hasSpawnedAbility == false)
                {
                    if (i == data.SelectedAmount - 1)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                    else if (RandomService.Value() >= 0.45f)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                }
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = data.SelectedSlot,
                    DistanceRatio = (i == 0) ? data.NextBatchDistance : 0,
                    MovementSpeed = data.MovementSpeed,
                    FinalValue = getProjectileValue.Invoke(i, data.SelectedAmount),
                    IsAbility = isAbility
                };

                batchData[i] = slotData;
            }
        }
        
        private struct SamePositionBatchData
        {
            public int SelectedSlot;
            public int SelectedAmount;
            public bool HasAbility;
            public float NextBatchDistance;
            public float MovementSpeed;
        }
        
        #endregion
        
        #region Random
        
        private void CreateRandomBatch(ref SlotData[] batchData, RandomBatchData data, Func<int,int,float> getProjectileValue)
        {
            var hasSpawnedAbility = false;
            var currentSlot = data.SelectedSlot;
            var slotAmount = _angleSlots;

            for (int i = 0; i < data.SelectedAmount; i++)
            {
                var isAbility = false;
                // Set if current projectile is ability
                if (data.HasAbility && hasSpawnedAbility == false)
                {
                    if (i == data.SelectedAmount - 1)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                    else if (RandomService.Value() >= 0.45f)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                }

                currentSlot += data.SlotRangeData.GetRandomRange() * GetRandomDirection();
                
                //Clamps Slot
                currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = currentSlot,
                    DistanceRatio = (i == 0) ? data.NextBatchDistance : data.InnerBatchDistance,
                    MovementSpeed = data.MovementSpeed,
                    FinalValue = getProjectileValue.Invoke(i, data.SelectedAmount),
                    IsAbility = isAbility
                };
                
                batchData[i] = slotData;
            }
        }
        
        private struct RandomBatchData
        {
            public int SelectedSlot;
            public int SelectedAmount;
            public bool HasAbility;
            public float NextBatchDistance;
            public float InnerBatchDistance;
            public float MovementSpeed;
            public SlotRangeData SlotRangeData;
        }
        
        #endregion

        #region UpAndDow

        private void CreateUpAndDownBatchBatch(ref SlotData[] batchData, UpAndDownBatchData data, Func<int,int,float> getProjectileValue)
        {

            var hasSpawnedAbility = false;
            var direction = GetRandomDirection();
            var currentSlot =  data.SelectedSlot;
            var slotAmount = _angleSlots;
            
            for (int i = 0; i < data.SelectedAmount; i++)
            {
                var isAbility = false;
                // Set if current projectile is ability
                if (data.HasAbility && hasSpawnedAbility == false)
                {
                    if (i == data.SelectedAmount - 1)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                    else if (RandomService.Value() >= 0.45f)
                    {
                        isAbility = true;
                        hasSpawnedAbility = true;
                    }
                }

                if (i < data.SelectedAmount - 1)
                {
                    currentSlot = data.SelectedSlot + (data.SlotRangeData.GetRandomRange() * direction);
                    direction *= -1;
                }
                
                //Clamps Slot
                currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = currentSlot,
                    DistanceRatio = (i == 0) ? data.NextBatchDistance : data.InnerBatchDistance,
                    MovementSpeed = data.MovementSpeed,
                    FinalValue = getProjectileValue.Invoke(i, data.SelectedAmount),
                    IsAbility = isAbility
                };
                
                batchData[i] = slotData;
            }
            
        }
        
        private struct UpAndDownBatchData
        {
            public int SelectedSlot;
            public int SelectedAmount;
            public bool HasAbility;
            public float NextBatchDistance;
            public float InnerBatchDistance;
            public float MovementSpeed;
            public SlotRangeData SlotRangeData;
        }

        #endregion
        
        private void CreateSingleProjectileBatch(ref SlotData[] batchData, SingleProjectileBatchData data, Func<int,int,float> getProjectileValue)
        {
            var slotData = new SlotData
            {
                Slot = data.SelectedSlot,
                DistanceRatio = data.NextBatchDistance,
                MovementSpeed = data.MovementSpeed,
                FinalValue = getProjectileValue.Invoke(0,1)
            };

            batchData[0] = slotData;
        }

        private struct SingleProjectileBatchData
        {
            public int SelectedSlot;
            public float NextBatchDistance;
            public float MovementSpeed;
        }

        private int GetRandomDirection() => RandomService.Value() > 0.5f ? -1 : 1;
        
    }
}