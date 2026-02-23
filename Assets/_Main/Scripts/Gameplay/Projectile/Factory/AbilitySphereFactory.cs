using System;
using System.Collections.Generic;
using _Main.Scripts.Common.SelectorByWeight;
using _Main.Scripts.EventBus;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Abilities;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.AbilitySphere;
using MeteorMadness.Gameplay.Abilities.So;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.GameConfig;
using UnityEngine;

namespace MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile
{
    //TODO: Ability Selector must be INDEPENDENT from the Level.
    //TODO: Ability Selector must be similar to spawn selector, having weights between abilities
    //TODO: Each time an ability is selected new weights are set
    //TODO: SO data must be re done, having base weights and  weights between abilities
    
    public class AbilitySphereFactory
    {
        #region Components
        
        #region Spawner

        private sealed class Spawner
        {
            private readonly GenericPool<AbilitySphereView> _pool;

            public int ActiveMeteorCount { get; private set; }

            public Spawner(AbilitySphereView meteorPrefab, int startCapacity = 3)
            {
                _pool = new GenericPool<AbilitySphereView>(meteorPrefab, startCapacity, 10, "Ability Sphere");
            }

            public IAbilitySphere Spawn()
            {
                var projectile = _pool.Get();

                ActiveMeteorCount++;

                projectile.OnRecycle += OnRecycleHandler;
                
                return projectile;
            }

            private void OnRecycleHandler(FlyingObjectView<AbilitySphereData> input)
            {
                input.OnRecycle -= OnRecycleHandler;
                _pool.Release((AbilitySphereView)input);
                ActiveMeteorCount--;
            }

            public void RecycleAll()
            {
                _pool.RecycleAll();
            }
        }

        #endregion

        #region Selector

        private class AbilitySelector
        {
            private Dictionary<AbilityType, int> _weightsDic;

            public AbilitySelector()
            {
                InitializeDic();
            }

            private void InitializeDic()
            {
                _weightsDic = new Dictionary<AbilityType, int>
                {
                    {AbilityType.SuperShield, 50},
                    {AbilityType.Health, 75},
                    {AbilityType.SlowMotion, 100},
                    {AbilityType.DoublePoints, 100},
                    {AbilityType.Automatic, 50},
                };
            }

            public AbilityType GetAbilityToAdd() => Roulette.Run(_weightsDic);
        }

        #endregion
        
        #endregion
        
        private readonly AbilitySelector _selector;
        private readonly Spawner _spawner;
        private readonly Func<bool> _doesDebugFunc;

        private bool _isStorageFull;
        private int _currentLevel;
        
        public AbilitySphereFactory(AbilitySphereView prefab, Func<bool> doesDebugFunc)
        {
            _doesDebugFunc = doesDebugFunc;
            _selector = new AbilitySelector();
            _spawner = new Spawner(prefab, 1);
        }
        
        private IAbilitySphere CreateAbilitySphere(ProjectileSpawnValues data)
        {
            var tempSphere = _spawner.Spawn();
            
            float angle = Mathf.Atan2(data.Direction.y, data.Direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            var ability = GetAbilityToAdd();
            
            tempSphere.SetValues(new AbilitySphereData
            {
                MovementSpeed = data.MovementSpeed,
                Rotation = tempRot,
                Position = data.Position,
                Direction = data.Direction.normalized,
                Ability = ability,
                Slot = data.Slot,
            });
            tempSphere.OnDeflection += DeflectionHandler;
            tempSphere.OnEarthCollision += OnEarthCollisionHandler;
            tempSphere.SetEnableMovement(true);
            
            
            if (tempSphere is IDebugAbilitySphere debug)
            {
                debug.DebugEnable = _doesDebugFunc.Invoke();
            }

            return tempSphere;
        }
        
        private AbilityType GetAbilityToAdd()
        {
            return _selector.GetAbilityToAdd();
        }

        public IAbilitySphere SpawnAbility(ProjectileSpawnValues data)
        {
            return CreateAbilitySphere(data);
        }

        public void RecycleAll()
        {
            _spawner.RecycleAll();
        }

        #region Handlers

        private void DeflectionHandler(IProjectile projectile, AbilitySphereCollisionData data)
        {
            if (projectile is IAbilitySphere sphere)
            {
                sphere.OnDeflection -= DeflectionHandler;
                sphere.OnEarthCollision -= OnEarthCollisionHandler;
            }
            
            AbilitiesEventCaller.Add(new AbilityAddData
            {
                AbilityType = data.Ability,
                Position = data.Position
            });
            
            ProjectileEventCaller.Deflected(new DeflectData
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Type = ProjectileType.AbilitySphere
            });
        }
        
        private void OnEarthCollisionHandler(IProjectile projectile, AbilitySphereCollisionData data)
        {
            if (projectile is IAbilitySphere sphere)
            {
                sphere.OnDeflection -= DeflectionHandler;
                sphere.OnEarthCollision -= OnEarthCollisionHandler;
            }
            
            ProjectileEventCaller.Collision(new CollisionData
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Type = ProjectileType.AbilitySphere
            });
        }

        #endregion
    }
}