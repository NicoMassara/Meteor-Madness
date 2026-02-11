using System;
using _Main.Scripts.Common.MyRandom;
using _Main.Scripts.Projectile;
using UnityEngine;


namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileBatchController : ProjectileBatchBase
    {
        private class AbilityCountdown
        {
            private int _currentCount;
            private bool _hasAbilityCountdownActive;
            private Vector2Int _abilityBatchRange = new Vector2Int(2, 5);
            
            public bool IsActive => _hasAbilityCountdownActive;
            
            public bool HasReachedTarget => _currentCount <= 0;

            public void CreateCountdown()
            {
                _currentCount = RandomService.Range(_abilityBatchRange.x, _abilityBatchRange.y+1);
                _hasAbilityCountdownActive = true;
            }

            public void DecreaseCountdown()
            {
                _currentCount--;
            }

            public void Restart()
            {
                _currentCount = int.MaxValue;
                _hasAbilityCountdownActive = false;
            }
        }
        
        private readonly int _levelAmount;
        private readonly IProjectileSpawnData _data;
        private readonly AbilityCountdown _abilityCountdown;
        private int _currentLevel;


        
        public ProjectileBatchController(IProjectileSpawnData data, ProjectileBatchData batchData, int levelAmount) 
            : base(batchData)
        {
            _data = data;
            _levelAmount =  levelAmount;
            _abilityCountdown =  new AbilityCountdown();
            InitializeData();
            OnDebugBatchCreated += debugData =>
            {
                ProjectileDebugEvents.TriggerBatchCreated(new DefaultBatchDebugData
                {
                    Level = _currentLevel,
                    Amount = debugData.Amount,
                    StartSlot = debugData.StartSlot,
                    Offset =  debugData.Offset,
                    InnerDist = debugData.InnerDist,
                    NextDist =  debugData.NextDist,
                    LastSlot =  debugData.LastSlot,
                    SpawnType =  debugData.SpawnType
                });
            };
        }

        private void InitializeData()
        {
            _currentLevel = 0;
            _abilityCountdown.Restart();
        }

        public override void Restart()
        {
            base.Restart();
            InitializeData();
        }

        public void SetLevel(int level)
        {
            _currentLevel = level;
        }
        
        protected override SlotData OverrideSlotData(SlotData data, int index, int batchAmount)
        {
            data.MovementSpeed *= _data.ProjectileSpeed; 
            data.FinalValue = GetBatchData().ProjectileValue;

            return data;
        }

        protected override BatchSpawnData GetSpawnData()
        {
            var hasAbility = false;
            
            if (_abilityCountdown.IsActive == false)
            {
                _abilityCountdown.CreateCountdown();
            }
            else
            {
                _abilityCountdown.DecreaseCountdown();

                if (_abilityCountdown.HasReachedTarget)
                {
                    _abilityCountdown.Restart();
                    //hasAbility = true;
                }
            }
            
            return _currentLevel < _levelAmount ? 
                CreateBatchSpawnData(_currentLevel, hasAbility) : 
                CreateBatchSpawnData(_levelAmount-1, hasAbility);
        }

        private BatchSpawnData CreateBatchSpawnData(int index, bool hasAbility)
        {
            var temp = _data.GetDataByIndex(index);
            var amount = temp.ProjectileAmountRange.RandomRange;

            return new BatchSpawnData
            {
                SpeedMultiplier = temp.SpeedMultiplierRange.RandomRange,
                Amount = amount,
                SlotRange = temp.SlotRange.RandomRange,
                InnerDistance = temp.InnerBatchDistanceRange.RandomRange,
                NextDistance = temp.NextBatchDistanceRange.RandomRange,
                Delay = temp.NextBatchDelayRange.RandomRange,
                NextSlotRange = temp.NextBatchSlotRange.RandomRange,
                SpawnType = amount == 1 ? SpawnType.None : GetSpawnType(temp.SpawnTypeRange),
                MinSlotRange = temp.SlotRange.Range.x,
                HasAbility = hasAbility,
                SlotRangeData = temp.SlotRange
            };
        }
    }
}