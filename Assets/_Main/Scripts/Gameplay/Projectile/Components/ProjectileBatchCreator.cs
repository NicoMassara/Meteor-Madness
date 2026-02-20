using System;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    public sealed class ProjectileBatchCreator
    {
        private readonly int _angleSlots;
        private SlotData[] _currentBatch;
        private int _lastSelectedSlot;
        private int _currentBatchIndex;
        private BatchSpawnData _lastSpawnData;

        public event Action<BatchDebugData> OnDebugBatchCreated;
        
        public ProjectileBatchCreator(int angleSlots)
        {
            _angleSlots = angleSlots;
            _lastSelectedSlot = -1;
        }

        public void RestartValues()
        {
            _currentBatchIndex = -1;
            _currentBatch = Array.Empty<SlotData>();
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
            
            if (selectedAmount == 1)
            {
                CreateSingleProjectileBatch(_currentBatch, new SingleProjectileBatchData
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
                        CreateRandomBatch(_currentBatch, new RandomBatchData
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
                        CreateAscendentBatchData(_currentBatch, new AscendentBatchData
                        {
                            SlotOffset =  spawnData.SlotRange,
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            IsAscendent = spawnData.SpawnType == SpawnType.Ascendent,
                            NextBatchDistance = spawnData.NextDistance,
                            MovementSpeed = spawnData.MovementSpeed,
                            FirstSpawnDistance = spawnData.InnerDistanceRangeData.GetRange().y,
                            InnerBatchDistance = spawnData.InnerDistanceRangeData.GetRange().x
                        }, getProjectileValue);
                        break;
                    
                    case SpawnType.SamePosition:
                        CreateSamePositionBatch(_currentBatch, new SamePositionBatchData
                        {
                            SelectedSlot = selectedSlot,
                            SelectedAmount = selectedAmount,
                            HasAbility = spawnData.HasAbility,
                            NextBatchDistance = spawnData.NextDistance,
                            MovementSpeed = spawnData.MovementSpeed,
                            InnerBatchDistance = spawnData.InnerDistance
                        }, getProjectileValue);
                        break;
                    
                    case SpawnType.UpAndDown:
                        CreateUpAndDownBatchBatch(_currentBatch, new UpAndDownBatchData
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
                Offset =  spawnData.SlotRange,
                InnerDist = innerBatchDistance,
                NextDist = nextBatchDistance,
                LastSlot = _lastSelectedSlot,
                SpawnType  = spawnData.SpawnType,
                Speed = spawnData.MovementSpeed
            });

            _lastSpawnData = spawnData;
            
            return selectedAmount;
        }
        public SlotData GetNextSlotData()
        {
            if (_currentBatchIndex < 0)
                throw new InvalidOperationException("No more slot data available.");
            
            var value = _currentBatch[_currentBatchIndex];
            _currentBatchIndex--;
            
            return value;
        }

        private bool TryAssignAbility(bool hasAbility, ref bool hasSpawnedAbility, int index)
        {
            if (hasAbility && hasSpawnedAbility == false)
            {
                if (index == 0 || RandomService.Value() >= 0.45f)
                {
                    hasSpawnedAbility = true;
                    return true;
                }
            }
            
            return false;
        }

        #region Ascendent/Descendent
        
        private void CreateAscendentBatchData(SlotData[] batchData, AscendentBatchData data, Func<int,int,float> getProjectileValue)
        {
            var hasSpawnedAbility = false;
            var slotAmount = _angleSlots;
            var isFirstProjectile = true;
            var isChained = _lastSpawnData.SpawnType is SpawnType.Ascendent or SpawnType.Descendent;
            var currentSlot = isChained ? _lastSelectedSlot : data.SelectedSlot;

            for (int i = data.SelectedAmount - 1; i >= 0; i--)
            {
                // Set if current projectile is ability
                var isAbility = TryAssignAbility(data.HasAbility, ref hasSpawnedAbility, i);
                
                var distanceRatio = 1f;

                // SLOT MOVEMENT
                if (!isFirstProjectile)
                {
                    int direction = data.IsAscendent ? 1 : -1;
                    currentSlot += direction * data.SlotOffset;
                    currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                }

                // DISTANCE
                if (isFirstProjectile && !isChained)
                {
                    distanceRatio = data.FirstSpawnDistance;
                }
                else
                {
                    distanceRatio = (i == 0) ? data.NextBatchDistance : data.InnerBatchDistance;
                }
                
                //Sets Slot Data
                var slotData = new SlotData
                {
                    Slot = currentSlot,
                    DistanceRatio = distanceRatio,
                    MovementSpeed = data.MovementSpeed,
                    FinalValue = getProjectileValue.Invoke(i, data.SelectedAmount),
                    IsAbility = isAbility
                };

                batchData[i] = slotData;
                
                isFirstProjectile = false;
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
            public float FirstSpawnDistance;
            public float MovementSpeed;
        }
        
        #endregion
        
        #region SamePosition

        // ReSharper disable once RedundantAssignment
        private void CreateSamePositionBatch(SlotData[] batchData, SamePositionBatchData data, Func<int,int,float> getProjectileValue)
        {
            var hasSpawnedAbility = false;
            var slotAmount = _angleSlots;

            for (int i = data.SelectedAmount - 1; i >= 0; i--)
            {
                // Set if current projectile is ability
                var isAbility = TryAssignAbility(data.HasAbility, ref hasSpawnedAbility, i);
                
                var step = (i + 1) / 2;             
                var direction = (i % 2 == 0) ? 1 : -1; 
                var offset = step * direction;

                var currentSlot = data.SelectedSlot + offset;
                
                //Clamps Slot
                currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                
                var slotData = new SlotData
                {
                    Slot = currentSlot,
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
            public float InnerBatchDistance;
            public float MovementSpeed;
        }
        
        #endregion
        
        #region Random
        
        private void CreateRandomBatch(SlotData[] batchData, RandomBatchData data, Func<int,int,float> getProjectileValue)
        {
            var hasSpawnedAbility = false;
            var currentSlot = data.SelectedSlot;
            var slotAmount = _angleSlots;
            var isFirstProjectile = true;

            for (int i = data.SelectedAmount - 1; i >= 0; i--)
            {
                // Set if current projectile is ability
                var isAbility = TryAssignAbility(data.HasAbility, ref hasSpawnedAbility, i);
                
                if (isFirstProjectile == false)
                {
                    currentSlot += data.SlotRangeData.GetRandomRange() * GetRandomDirection();
                    
                    //Clamps Slot
                    currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                }
                
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
                
                isFirstProjectile = false;
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

        private void CreateUpAndDownBatchBatch(SlotData[] batchData, UpAndDownBatchData data, Func<int,int,float> getProjectileValue)
        {
            var hasSpawnedAbility = false;
            var direction = GetRandomDirection();
            var currentSlot =  data.SelectedSlot;
            var slotAmount = _angleSlots;
            var isFirstProjectile = true;

            for (int i = data.SelectedAmount - 1; i >= 0; i--)
            {
                // Set if current projectile is ability
                var isAbility = TryAssignAbility(data.HasAbility, ref hasSpawnedAbility, i);

                if (isFirstProjectile == false)
                {
                    currentSlot = data.SelectedSlot + (data.SlotRangeData.GetRandomRange() * direction);
                    direction *= -1;
                    
                    //Clamps Slot
                    currentSlot = ((currentSlot % slotAmount) + slotAmount) % slotAmount;
                }
                
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
                
                isFirstProjectile = false;
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
        
        private void CreateSingleProjectileBatch(SlotData[] batchData, SingleProjectileBatchData data, Func<int,int,float> getProjectileValue)
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