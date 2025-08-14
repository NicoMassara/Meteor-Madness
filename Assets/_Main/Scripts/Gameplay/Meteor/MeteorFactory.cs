using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;


namespace _Main.Scripts.Gameplay.Meteor
{
    public class MeteorFactory : MonoBehaviour
    {
        [SerializeField] private Meteor meteorPrefab;
        [SerializeField] private Transform centerOfGravity;


        
        private ObjectPool<Meteor> _pool;

        public UnityAction OnShieldHit;
        public UnityAction OnEarthHit;

        private void Start()
        {
            _pool = new ObjectPool<Meteor>(
                createFunc: () => Instantiate(meteorPrefab),
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: b => Destroy(b.gameObject),
                collectionCheck: true,
                defaultCapacity: 20,
                maxSize: 500
            );
        }

        private void OnGet(Meteor meteor)
        {
            meteor.gameObject.SetActive(true);
        }

        private void OnRelease(Meteor meteor)
        {
            meteor.gameObject.SetActive(false);
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
        }

        private void Meteor_OnRecycleHandler(Meteor meteor)
        {
            meteor.OnRecycle -= Meteor_OnRecycleHandler;
            _pool.Release(meteor);
        }

        private void Meteor_OnHitHandler(Meteor meteor, bool hasHitShield)
        {
            meteor.OnHit-= Meteor_OnHitHandler;
            if (hasHitShield)
            {
                OnShieldHit?.Invoke();
            }
            else
            {
                OnEarthHit?.Invoke();
            }
        }
    }
}