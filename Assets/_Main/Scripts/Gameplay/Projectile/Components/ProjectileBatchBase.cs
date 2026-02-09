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

            public int MinSlotRange;
        }

        public struct ProjectileBatchData
        {
            public int SlotAmount;
            public int ProjectileValue;
        }

        protected readonly ProjectileBatchData _batchData;
        private SlotData[] _currentBatch;
        private int _lastSelectedSlot;
        private int _currentBatchIndex;

        protected ProjectileBatchData GetBatchData() => _batchData;
        

        public ProjectileBatchBase(ProjectileBatchData data)
        {
            _lastSelectedSlot = -1;
        }

        public virtual void Restart()
        {
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
                var currentOffset = 0;
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
                    var isAbility = false;
                    
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

                        currentSlot = (int)Mathf.Repeat(currentSlot, GetBatchData().SlotAmount);
                    }

                    var slotData = new SlotData
                    {
                        Slot = currentSlot,
                        DistanceRatio = (i == 0) ? nextBatchDistance : innerBatchDistance,
                        MovementSpeed = spawnData.SpeedMultiplier
                    };
                    
                    _currentBatch[i] = OverrideSlotData(slotData, i + 1, selectedAmount);
                }

                _lastSelectedSlot = _currentBatch[0].Slot;
                _currentBatchIndex = selectedAmount - 1;
            }
            
            return selectedAmount;
        }

        public SlotData GetNextSlotData()
        {
            var value = _currentBatch[_currentBatchIndex];
            _currentBatchIndex--;
            
            return value;
        }

        protected abstract SlotData OverrideSlotData(SlotData data,int index, int batchAmount);

        private int GetRandomDirection() => RandomService.Value() > 0.5f ? -1 : 1;
        
        protected abstract BatchSpawnData GetSpawnData();
    }
}