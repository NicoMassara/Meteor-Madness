using System;
using System.Collections.Generic;
using _Main.Scripts.Particles;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorFactory : MonoBehaviour
    {
        [FormerlySerializedAs("meteorPrefab")] [SerializeField] private MeteorMotor meteorMotorPrefab;
        [SerializeField] private Transform centerOfGravity;
        
        private readonly List<MeteorMotor> _activeMeteors = new List<MeteorMotor>();
        private GenericPool<MeteorMotor> _pool;

        public UnityAction<Vector3> OnShieldHit;
        public UnityAction<Vector3, Quaternion> OnEarthHit;

        private void Start()
        {
            _pool = new GenericPool<MeteorMotor>(meteorMotorPrefab);
        }

        public void SpawnMeteor(float meteorSpeed, Vector2 spawnPosition)
        {
            Vector2 direction = (Vector2)centerOfGravity.position - spawnPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            var tempMeteor = _pool.Get();
            tempMeteor.SetValues(meteorSpeed, tempRot, spawnPosition);
            tempMeteor.OnHit += Meteor_OnHitHandler;
            tempMeteor.OnRecycle += Meteor_OnRecycleHandler;
            _activeMeteors.Add(tempMeteor);
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
            meteorMotor.OnHit -= Meteor_OnHitHandler;
            meteorMotor.OnRecycle -= Meteor_OnRecycleHandler;
            _activeMeteors.Remove(meteorMotor);
            _pool.Release(meteorMotor);
        }

        private void Meteor_OnHitHandler(MeteorMotor meteorMotor, bool hasHitShield)
        {
            meteorMotor.OnHit-= Meteor_OnHitHandler;
            if (hasHitShield)
            {
                OnShieldHit?.Invoke(meteorMotor.transform.position);
            }
            else
            {
                OnEarthHit?.Invoke(meteorMotor.transform.position, meteorMotor.transform.rotation);
            }
        }
    }
}