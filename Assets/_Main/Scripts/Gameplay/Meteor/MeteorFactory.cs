using System;
using System.Collections.Generic;
using _Main.Scripts.Particles;
using UnityEngine;
using UnityEngine.Events;


namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorFactory : MonoBehaviour
    {
        [SerializeField] private Meteor meteorPrefab;
        [SerializeField] private Transform centerOfGravity;
        
        private readonly List<Meteor> _activeMeteors = new List<Meteor>();
        private GenericPool<Meteor> _pool;

        public UnityAction<Vector3> OnShieldHit;
        public UnityAction<Vector3, Quaternion> OnEarthHit;

        private void Start()
        {
            _pool = new GenericPool<Meteor>(meteorPrefab);
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

        private void Meteor_OnRecycleHandler(Meteor meteor)
        {
            meteor.OnHit -= Meteor_OnHitHandler;
            meteor.OnRecycle -= Meteor_OnRecycleHandler;
            _activeMeteors.Remove(meteor);
            _pool.Release(meteor);
        }

        private void Meteor_OnHitHandler(Meteor meteor, bool hasHitShield)
        {
            meteor.OnHit-= Meteor_OnHitHandler;
            if (hasHitShield)
            {
                OnShieldHit?.Invoke(meteor.transform.position);
            }
            else
            {
                OnEarthHit?.Invoke(meteor.transform.position, meteor.transform.rotation);
            }
        }
    }
}