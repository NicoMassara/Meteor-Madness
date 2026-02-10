using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;

namespace _Main.Scripts.Gameplay.Projectile.Components
{
    public class ProjectileRingController : ProjectileBatchBase
    {
        private readonly IProjectileRingData _data;
        private SlotData[] _currentBatch;
        private int _currentBatchIndex;
        

        public ProjectileRingController(IProjectileRingData data, ProjectileBatchData batchData) 
            : base(batchData)
        {
            _data = data;
        }

        protected override SlotData OverrideSlotData(SlotData data, int index, int batchAmount)
        {
            data.FinalValue = index % 2 == 0 ? CalculateValuePerMeteor(batchAmount) : 0;
            data.MovementSpeed *= _data.ProjectileSpeed; 
            
            return data;
        }

        protected override BatchSpawnData GetSpawnData()
        {
            var temp = _data.RingData;
            
            return new BatchSpawnData
            {
                SpeedMultiplier = temp.SpeedMultiplierRange.RandomRange,
                Amount = temp.ProjectileAmountRange.RandomRange,
                SlotRange = temp.SlotRange.RandomRange,
                InnerDistance = temp.InnerBatchDistanceRange.RandomRange,
                NextDistance = temp.NextBatchDistanceRange.RandomRange,
                Delay = temp.NextBatchDelayRange.RandomRange,
                NextSlotRange = temp.NextBatchSlotRange.RandomRange,
                SpawnType = GetSpawnType(temp.SpawnTypeRange),
                MinSlotRange = temp.SlotRange.Range.x
            };
        }

        private float CalculateValuePerMeteor(int batchAmount)
        {
            var batchValue = GetBatchData().ProjectileValue * 5;
            var finalValue = batchValue / batchAmount;
            return finalValue;
        }
    }
}