using System;
using System.Collections.Generic;
using _Main.Scripts.Particles;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorFactory
    {
        private readonly MeteorMotor _meteorPrefab;
        private readonly Transform _centerOfGravity;
        
        private readonly List<MeteorMotor> _activeMeteors = new List<MeteorMotor>();
        private GenericPool<MeteorMotor> _pool;

        public UnityAction<Vector3> OnShieldHit;
        public UnityAction<Vector3, Quaternion> OnEarthHit;

        public MeteorFactory(MeteorMotor meteorPrefab, Transform centerOfGravity)
        {
            this._meteorPrefab = meteorPrefab;
            this._centerOfGravity = centerOfGravity;
            
            Initialize();
        }

        private void Initialize()
        {
            _pool = new GenericPool<MeteorMotor>(_meteorPrefab);
        }

        // ReSharper disable Unity.PerformanceAnalysis

        public MeteorMotor SpawnMeteor()
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

        private void Meteor_OnRecycleHandler(MeteorMotor meteorMotor)
        {
            meteorMotor.OnHit = null;
            meteorMotor.OnRecycle -= Meteor_OnRecycleHandler;
            _activeMeteors.Remove(meteorMotor);
            _pool.Release(meteorMotor);
        }
    }
}