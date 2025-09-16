using System.Collections.Generic;
using _Main.Scripts.Particles;
using UnityEngine;
using UnityEngine.Events;


namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorFactory
    {
        private readonly MeteorView _meteorPrefab;
        
        private readonly List<MeteorView> _activeMeteors = new List<MeteorView>();
        private GenericPool<MeteorView> _pool;
        public int ActiveMeteorCount => _activeMeteors.Count;

        public UnityAction<Vector3> OnShieldHit;
        public UnityAction<Vector3, Quaternion> OnEarthHit;

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
            tempMeteor.OnRecycle += Meteor_OnRecycleHandler;
            _activeMeteors.Add(tempMeteor);
            return tempMeteor;
        }

        public void RecycleAll()
        {
            for (int i = _activeMeteors.Count - 1; i >= 0; i--)
            {
                _activeMeteors[i].ForceRecycle();
            }
        }

        private void Meteor_OnRecycleHandler(MeteorView meteorMotorOld)
        {
            meteorMotorOld.OnDeflection = null;
            meteorMotorOld.OnEarthCollision = null;
            meteorMotorOld.OnRecycle -= Meteor_OnRecycleHandler;
            _activeMeteors.Remove(meteorMotorOld);
            _pool.Release(meteorMotorOld);
        }
    }
}