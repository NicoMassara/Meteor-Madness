using System;
using System.Collections;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.Meteor;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.GameConfig;
using UnityEngine;

namespace _Main.Scripts.Projectile
{
    internal class MeteorFactory
    {
        #region Components
        
        #region Spawner

        private sealed class Spawner
        {
            private readonly GenericPool<MeteorView> _pool;

            public int ActiveMeteorCount { get; private set; }

            public Spawner(MeteorView meteorPrefab, int startCapacity = 3)
            {
                _pool = new GenericPool<MeteorView>(meteorPrefab, startCapacity, 10, "Meteor");
            }

            public IMeteor Spawn()
            {
                var meteor = _pool.Get();

                ActiveMeteorCount++;
                meteor.OnRecycle += OnRecycleHandler;
                meteor.SetEnableMovement(false);
                
                return meteor;
            }

            private void OnRecycleHandler(FlyingObjectView<MeteorData> input)
            {
                input.OnRecycle -= OnRecycleHandler;
                _pool.Release((MeteorView)input);
                ActiveMeteorCount--;
            }

            public void RecycleAll()
            {
                _pool.RecycleAll();
            }
        }

        #endregion
        
        #endregion
        
        private readonly Spawner _spawner;
        private readonly Func<bool> _doesDebugFunc;

        public MeteorFactory(MeteorView meteorPrefab,
            Func<bool> doesDebugFunc)
        {
            _doesDebugFunc = doesDebugFunc;
            _spawner = new Spawner(meteorPrefab, 5);
        }
        
        #region Spawn
        
        private IMeteor CreateMeteor(ProjectileSpawnValues data)
        {
            var meteor = _spawner.Spawn();
            float angle = Mathf.Atan2(data.Direction.y, data.Direction.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            meteor.SetValues(new MeteorData
            {
                MovementSpeed = data.MovementSpeed,
                Rotation = rotation,
                Position = data.Position,
                Direction = data.Direction.normalized,
                Value = data.Value,
                Slot = data.Slot
            });

            meteor.OnDeflection += Meteor_OnDeflectionHandler;
            meteor.OnEarthCollision += Meteor_OnCollisionHandler;
            
            if (meteor is IDebugMeteor debug)
            {
                debug.DebugEnable = _doesDebugFunc.Invoke();
            }
            
            return meteor;
        }

        public IMeteor SpawnMeteor(ProjectileSpawnValues data) => CreateMeteor(data);

        #endregion
        
        public void RecycleAll()
        {
            _spawner.RecycleAll();
        }
        
        #region Handlers

        private void Meteor_OnCollisionHandler(IProjectile input1, MeteorCollisionData data)
        {
            if (input1 is IMeteor meteor)
            {
                meteor.OnDeflection -= Meteor_OnDeflectionHandler;
                meteor.OnEarthCollision -= Meteor_OnCollisionHandler;
            }
            
            ProjectileEventCaller.Collision(new CollisionData
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Type = ProjectileType.Meteor
            });
        }

        private void Meteor_OnDeflectionHandler(IProjectile input1, MeteorCollisionData data)
        {
            if (input1 is IMeteor meteor)
            {
                meteor.OnDeflection -= Meteor_OnDeflectionHandler;
                meteor.OnEarthCollision -= Meteor_OnCollisionHandler;
            }
            
            ProjectileEventCaller.Deflected(new DeflectData
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Value = data.Value,
                Type = ProjectileType.Meteor
            });
        }

        #endregion
    }
}