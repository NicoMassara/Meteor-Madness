using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileSlotSelector
    {
        private const int SlotsAmount = GameParameters.GameplayValues.AngleSlots;
        private readonly IProjectileSpawnData _data;
        private SlotData[] _currentBatch;
        private int _currentBatchIndex;
        private int _currentLevel;
        private int _lastSelectedSlot = -1;
        
        //TODO: Try Pre Cache the data before every level
        
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
            
            var spawnType = spawnData.SpawnTypeRange.RandomRange;
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