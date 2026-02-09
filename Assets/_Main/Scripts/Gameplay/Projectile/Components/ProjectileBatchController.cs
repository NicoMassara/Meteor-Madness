using _Main.Scripts.Projectile;


namespace _Main.Scripts.Gameplay.Projectile.Components
{
    internal class ProjectileBatchController : ProjectileBatchBase
    {
        private readonly int _levelAmount;
        private readonly IProjectileSpawnData _data;
        private readonly SpawnTypeSelector _spawnTypeSelector;
        private int _currentLevel;
        
        public ProjectileBatchController(IProjectileSpawnData data, ProjectileBatchData batchData, int levelAmount) 
            : base(batchData)
        {
            _data = data;
            _spawnTypeSelector = new SpawnTypeSelector();
            _levelAmount =  levelAmount;
            
            InitializeData();
        }

        private void InitializeData()
        {
            _currentLevel = 0;
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
            data.IsAbility = false;
            //TODO: Set ability Random
            
            return data;
        }

        protected override BatchSpawnData GetSpawnData()
        {
            return _currentLevel < _levelAmount ? 
                CreateBatchSpawnData(_currentLevel) : 
                CreateBatchSpawnData(_levelAmount-1);
        }

        private BatchSpawnData CreateBatchSpawnData(int index)
        {
            var temp = _data.GetDataByIndex(index);
            
            return new BatchSpawnData
            {
                SpeedMultiplier = temp.SpeedMultiplierRange.RandomRange,
                Amount = temp.ProjectileAmountRange.RandomRange,
                SlotRange = temp.SlotRange.RandomRange,
                InnerDistance = temp.InnerBatchDistanceRange.RandomRange,
                NextDistance = temp.NextBatchDistanceRange.RandomRange,
                Delay = temp.NextBatchDelayRange.RandomRange,
                NextSlotRange = temp.NextBatchSlotRange.RandomRange,
                SpawnType = _spawnTypeSelector.GetSpawnType(temp.SpawnTypeRange),
                MinSlotRange = temp.SlotRange.Range.x
            };
        }
    }
}