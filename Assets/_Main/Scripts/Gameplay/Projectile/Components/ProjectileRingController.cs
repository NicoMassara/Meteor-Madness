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
            data.MovementSpeed *= 60; 
            
            return data;
        }

        protected override BatchSpawnData GetSpawnData()
        {
            //TODO: Generate Data using SO and Interface
            return new BatchSpawnData();
        }

        private int CalculateValuePerMeteor(int batchAmount)
        {
            var batchValue = GetBatchData().ProjectileValue * 5;
            var finalValue = batchValue / batchAmount;
            return finalValue;
        }
    }
}