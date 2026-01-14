using System;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.Gameplay.Particles;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.ProjectileObject
{
    public class ProjectileParticles<T> : MonoBehaviour
        where T : ProjectileCollisionData
    {
        [SerializeField] private ParticleDataSo collisionParticleData;
        [SerializeField] private ParticleDataSo deflectionParticleData;

        protected IParticleProjectile<T> Projectile { get; private set;}

        private void Awake()
        {
            Projectile = GetComponent<IParticleProjectile<T>>();
            if (Projectile == null)
            {
                Debug.LogWarning("Projectile object not found");
                enabled = false;
            }
        }

        private void Start()
        {
            Projectile.OnDeflection += OnDeflectionHandler;
            Projectile.OnEarthCollision += OnCollisionHandler;
        }

        protected virtual void OnCollisionHandler(IProjectile projectile, T data)
        {
            ParticleEventCaller.Spawn(new ParticleSpawnData
            {
                Position = data.Position,
                ParticleData = collisionParticleData,
                MoveDirection = data.Direction
                    
            });
        }

        protected virtual void OnDeflectionHandler(IProjectile projectile, T data)
        {
            ParticleEventCaller.Spawn(new ParticleSpawnData
            {
                Position = data.Position,
                ParticleData = deflectionParticleData,
                MoveDirection = data.Direction
                    
            });
        }
    }
}