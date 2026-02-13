using System;
using System.Collections.Generic;
using _Main.Scripts.EventBus;
using _Main.Scripts.Projectile;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject;
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
            public bool IsStorageFull { get; set; }
            
            private readonly Func<Tuple<AbilityType[], int[]>> _getValuesAction;
            private readonly Func<Tuple<int[],AbilityType[]>> _getUnlockAction;
            private readonly Dictionary<AbilityType, ActionValue> _multipliers = new Dictionary<AbilityType, ActionValue>();
            private AbilityType _abilityToDrop;

            private class ActionValue
            {
                public readonly AbilityType AbilityType;
                public int Value { get; private set; }

                public ActionValue(AbilityType abilityType)
                {
                    AbilityType = abilityType;
                }

                public void IncreaseValue() => Value += 5;
                public void DecreaseValue() => Value -= 5;
                public void EnableValue() => Value = 10;
                public void DisableValue() => Value = 0;
            }

            public AbilitySelector(
                Func<Tuple<AbilityType[], int[]>> getValuesAction, 
                Func<Tuple<int[],AbilityType[]>> getUnlockAction)
            {
                _getValuesAction = getValuesAction;
                _getUnlockAction = getUnlockAction;

                for (int i = 0; i < (int)AbilityType.Default_MAX; i++)
                {
                    var ability = (AbilityType)i;
                    _multipliers.Add(ability,new ActionValue(ability));
                }
            }

            private AbilityType GetAbilityToUnlock(int level)
            {
                var tempValues = _getUnlockAction();
                var length = tempValues.Item1.Length;
                
                for (int i = 0; i < length; i++)
                {
                    var unlockLevel = tempValues.Item1[i];

                    if (unlockLevel == level)
                    {
                        return tempValues.Item2[i];
                    }
                }

                return AbilityType.None;
            }

            private void ResetMultipliers()
            {
                foreach (var item in _multipliers)
                {
                    item.Value.DisableValue();
                }
            }

            private int GetAbilityValue(AbilityType abilityType)
            {
                if (_multipliers.TryGetValue(abilityType, out var multiplier))
                {
                    return multiplier.Value;
                }

                return 1;
            }

            public void IncreaseValue(AbilityType ability)
            {
                if (_multipliers.TryGetValue(ability, out var multiplier))
                {
                    multiplier.IncreaseValue();
                }
            }

            public void DecreaseValue(AbilityType ability)
            {
                if (_multipliers.TryGetValue(ability, out var multiplier))
                {
                    multiplier.DecreaseValue();
                }
            }

            public void UpdateLevel(int level)
            {
                var ability = GetAbilityToUnlock(level);
                if (ability == AbilityType.None) return;

                if (_multipliers.TryGetValue(ability, out var multiplier))
                {
                    multiplier.EnableValue();
                }
            }
            
            public void Reset() => ResetMultipliers();

            public AbilityType GetAbilityToAdd()
            {
                if (_abilityToDrop == AbilityType.None)
                {
                    var tempDic = new Dictionary<AbilityType, int>();
                    var values = _getValuesAction();
                    var length = values.Item1.Length;

                    for (int i = 0; i < length; i++)
                    {
                        var ability = values.Item1[i];
                        var finalValue = values.Item2[i] * GetAbilityValue(ability);

                        tempDic.Add(ability, finalValue);
                    }

                    _abilityToDrop = Roulette.Run(tempDic);

                    return _abilityToDrop;
                }

                var tempValue = _abilityToDrop;
                _abilityToDrop = AbilityType.None;
            
                return tempValue;
            }

        public void SetAbilityToDrop(AbilityType ability) => _abilityToDrop = ability;
    }

        #endregion
        
        #endregion
        
        private readonly AbilitySelector _selector;
        private readonly Spawner _spawner;
        private readonly Func<bool> _doesDebugFunc;
        private bool _isStorageFull;
        private int _currentLevel;
        
        public AbilitySphereFactory(AbilitySphereView prefab, AbilitySelectorDataSo selectorData, Func<bool> doesDebugFunc)
        {
            _doesDebugFunc = doesDebugFunc;
            _selector = new AbilitySelector(selectorData.GetRarityValues,selectorData.GetUnlockLevelValues);
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