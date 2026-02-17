using _Main.Scripts.Projectile;

namespace _Main.Scripts.Gameplay.Projecitle.Spawner
{
    internal abstract class ProjectileBatchControllerBase<T> : IBatchTypeCreator
    where T : IBatchDataBase
    {

        protected readonly ISpawnWeightsData SpawnWeights;
        protected readonly T BatchData;
        protected readonly float ProjectileBaseValue;

        protected ProjectileBatchControllerBase(T data, ISpawnWeightsData spawnWeights, float projectileBaseValue)
        {
            BatchData = data;
            SpawnWeights = spawnWeights;
            ProjectileBaseValue = projectileBaseValue;
        }

        public abstract BatchSpawnData GetBatchSpawnData(SelectRandomSpawnDelegate selectRandomSpawn);
        
        public abstract float GetProjectileValue(int index, int batchAmount);
        public abstract void RestartValues();
    }
}