using System.Collections.Generic;

namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorFactory
    {
        private readonly MeteorView _meteorPrefab;
        
        private readonly List<MeteorView> _activeMeteors = new List<MeteorView>();
        private GenericPool<MeteorView> _pool;
        public int ActiveMeteorCount => _activeMeteors.Count;

        public MeteorFactory(MeteorView meteorPrefab)
        {
            this._meteorPrefab = meteorPrefab;
            
            Initialize();
        }

        private void Initialize()
        {
            _pool = new GenericPool<MeteorView>(_meteorPrefab);
        }

        // ReSharper disable Unity.PerformanceAnalysis

        public MeteorView SpawnMeteor()
        {
            var tempMeteor = _pool.Get();
            tempMeteor.OnRecycle += OnRecycleHandler;
            _activeMeteors.Add(tempMeteor);
            return tempMeteor;
        }

        public void RecycleAll()
        {
            _pool.RecycleAll();
        }

        private void OnRecycleHandler(MeteorView meteorMotor)
        {
            meteorMotor.OnDeflection = null;
            meteorMotor.OnEarthCollision = null;
            meteorMotor.OnRecycle -= OnRecycleHandler;
            _activeMeteors.Remove(meteorMotor);
            _pool.Release(meteorMotor);
        }
    }
}