using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileSlotSelector
    {
        private const int SlotsAmount = GameParameters.GameplayValues.SpawnLevelAmount;
        private readonly IProjectileSpawnData _data;
        private SlotData[] _currentBatch;
        private int _currentBatchIndex;
        private int _currentLevel;
        private int _lastSelectedSlot = -1;
        
        public ProjectileSlotSelector(IProjectileSpawnData data)
        {
            _data = data;
            _lastSelectedSlot = -1;
        }

        public void Restart()
        {
            _currentLevel = 0;
            _lastSelectedSlot = -1;
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;
        }

        public int CreateBatchData()
        {
            var spawnData = _data.GetDataByIndex(_currentLevel);

            var selectedSlot = 0;
            
            if (_lastSelectedSlot == -1)
            {
                selectedSlot = Random.Range(0, SlotsAmount);
            }
            else if (_lastSelectedSlot > -1)
            {
                var slotRange = GetRandomDirection() * spawnData.NextBatchSlotRange.RandomRange;
                selectedSlot = _lastSelectedSlot + slotRange;
            }
            
            var spawnType = spawnData.SpawnTypeRange.RandomRange;
            var selectedAmount = spawnData.AmountRange.RandomRange;
            var innerBatchDistance = spawnData.InnerBatchDistanceRange.RandomRange;

            _currentBatch = new SlotData[selectedAmount];

            for (var i = selectedAmount - 1; i >= 0; i--)
            {
                if (i != selectedAmount - 1)
                {
                    var offset = spawnData.SlotRange.RandomRange;

                    switch (spawnType)
                    {
                        case SpawnType.Random:
                            offset *= GetRandomDirection();
                            selectedSlot += offset;
                            break;

                        case SpawnType.Ascendent:
                            selectedSlot += offset;
                            break;

                        case SpawnType.Descendent:
                            selectedSlot -= offset;
                            break;
                    }

                    selectedSlot = (int)Mathf.Repeat(selectedSlot, SlotsAmount);
                }

                _currentBatch[i] = new SlotData
                {
                    Slot = selectedSlot,
                    DistanceRatio = (i == 0) ? 
                        spawnData.NextBatchDistanceRange.RandomRange : 
                        innerBatchDistance
                };
            }

            _lastSelectedSlot = _currentBatch[0].Slot;
            _currentBatchIndex = selectedAmount - 1;
            return selectedAmount;
        }
        
        public SlotData GetNextSlotData()
        {
            var value = _currentBatch[_currentBatchIndex];
            _currentBatchIndex--;
            
            return value;
        }

        private int GetRandomDirection() => Random.value > 0.5f ? -1 : 1;
    }
}