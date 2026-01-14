using System;
using System.Collections.Generic;
using _Main.Scripts.EventBus;
using MeteorMadness.Contracts;
using MeteorMadness.Contracts.Events;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.Core.FlyingObject;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile;
using MeteorMadness.Gameplay._Main.Scripts.Gameplay.Projectile.AbilitySphere;
using MeteorMadness.Gameplay.Abilities.So;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.GameConfig;
using NicolasMassara.CustomTimerManager;
using UnityEngine;

namespace _Main.Scripts.Projectile
{
    public class AbilitySphereFactory : MonoBehaviour
    {
        #region Components
        
        #region Spawner

        private sealed class Spawner
        {
            private readonly GenericPool<AbilitySphereView> _pool;

            public int ActiveMeteorCount { get; private set; }

            public Spawner(AbilitySphereView meteorPrefab, int startCapacity = 3)
            {
                _pool = new GenericPool<AbilitySphereView>(meteorPrefab, startCapacity, 10, "Meteor");
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

            private readonly Roulette _roulette = new Roulette();
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

                    _abilityToDrop = _roulette.Run(tempDic);

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
        
        [Header("Components")]
        [SerializeField] private AbilitySphereView prefab;
        [SerializeField] private AbilitySelectorDataSo selectorData;
        [Header("Values")] 
        [Range(5, 15f)] 
        [SerializeField] private float spawnDelay = 5f;
        [Header("Debug")] 
        [SerializeField] private bool doesDebug;
        
        private bool _hasTimerEnable;
        private bool _isGameplayActive;
        private bool _isStorageFull;
        private bool _isTimerRunning;
        private int _minUnlockLevel;
        private int _currentLevel;
        private TimerManager.GeneratedId _spawnTimerId;
        private AbilitySelector _selector;
        private Spawner _spawner;
        
        private void Awake()
        {
            SetEventBus();

            BootEvents.OnSubSystemRequestInitialize += Initialize;
        }
        
        private void Initialize()
        {
            BootEvents.OnSubSystemRequestInitialize -= Initialize;
            //
            _minUnlockLevel = selectorData.MinUnlockLevel;
            
            _selector = new AbilitySelector(selectorData.GetRarityValues,selectorData.GetUnlockLevelValues);
            _spawner = new Spawner(prefab, 1);
            
            
            BootEvents.SubSystemInitialized();
        }
        private void SendAbility()
        {
            AbilitiesEventCaller.RequestSpawn();
        }
        
        private void CreateAbilitySphere(ProjectileSpawnData data)
        {
            var movementSpeed = GameConfigManager.Instance.GetGameplayData().ProjectileData.MaxProjectileSpeed 
                                * data.MovementMultiplier;
            var tempSphere = _spawner.Spawn();
            
            float angle = Mathf.Atan2(data.Direction.y, data.Direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            var ability = GetAbilityToAdd();
            
            tempSphere.SetValues(new AbilitySphereData()
            {
                MovementSpeed = movementSpeed,
                Rotation = tempRot,
                Position = data.Position,
                Direction = data.Direction.normalized,
                Ability = ability
            });
            tempSphere.OnDeflection += DeflectionHandler;
            tempSphere.OnEarthCollision += OnEarthCollisionHandler;
            tempSphere.SetEnableMovement(true);
            
            Debug.LogWarning(ability);
            
            if (tempSphere is IDebugAbilitySphere debug)
            {
                debug.DebugEnable = doesDebug;
            }

            if (tempSphere is IProjectile projectile)
            {
                ProjectileEventCaller.Add((projectile));
            }
            else
            {
                Debug.LogWarning($"Projectile type {tempSphere} does not implement {nameof(IProjectile)}");
                tempSphere.Recycle();
            }
        }
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
            
            var temp = UnityEngine.Random.Range(spawnDelay, spawnDelay * 1.15f);
            temp = _isStorageFull ? temp/2 : temp;
            
            TryRunTimer(temp);
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
            
            var temp = UnityEngine.Random.Range(spawnDelay * 0.75f, spawnDelay);
            temp = _isStorageFull ? temp/2 : temp;
            TryRunTimer(temp);
        }
        private void SetTimer(float time)
        {
            _isTimerRunning = true;
            _spawnTimerId = TimerManager.Add(new TimerData(time,() =>
            {
                SendAbility();
                _isTimerRunning = false;
            }));
        }
        private void RemoveTimer(TimerManager.GeneratedId timerId)
        {
            if(timerId == null) return;   
            
            if (timerId.IsActive)
            {
                TimerManager.Remove(timerId);
            }
        }
        private void PauseTimer(TimerManager.GeneratedId timerId)
        {
            if(timerId == null) return;   
            
            if (timerId.IsActive)
            {
                TimerManager.Pause(timerId);
            }
        }
        private void ResumeTimer(TimerManager.GeneratedId timerId)
        {
            if(timerId == null) return;   
            
            if (timerId.IsActive)
            {
                TimerManager.Resume(timerId);
            }
        }

        private void TryRunTimer(float time)
        {
            if (_isTimerRunning)
            {
                Debug.Log("Ability Timer already running");
            }
            else
            {
                Debug.Log($"Ability Timer Set To: {time}");
                SetTimer(time);
            }
        }
        
        private AbilityType GetAbilityToAdd()
        {
            return _selector.GetAbilityToAdd();
        }
        

        #region EventBus

        private void SetEventBus()
        {
            ProjectileEventSubscriber.Spawn(EventBus_Projectile_Spawn);
            ProjectileEventSubscriber.DisableSpawn(EventBus_Projectile_DisableSpawn);
            ProjectileEventSubscriber.EnableSpawn(EventBus_Projectile_EnableSpawn);
            ProjectileEventSubscriber.UpdateLevel(EventBus_Projectile_UpdateLevel);
            //
            AbilitiesEventSubscriber.SetStorageFull(EventBus_Ability_StorageFull);
            AbilitiesEventSubscriber.NotifyIsActive(EventBus_Ability_SetActive);
            AbilitiesEventSubscriber.Add(EventBus_Ability_Add);
            AbilitiesEventSubscriber.SetNextSpawn(EventBus_Ability_NextSpawn);
            //
            GameModeEventSubscriber.SetPause(EventBus_GameMode_SetPause);
        }

        private void EventBus_GameMode_SetPause(GameModeEvents.SetPause input)
        {
            if (input.IsPaused)
            {
                PauseTimer(_spawnTimerId);
            }
            else
            {
                ResumeTimer(_spawnTimerId);
            }
        }

        #region Ability

        private void EventBus_Ability_NextSpawn(AbilitiesEvents.SetNextSpawn input)
        {
            _selector.SetAbilityToDrop(input.AbilityType);
        }

        private void EventBus_Ability_StorageFull(AbilitiesEvents.SetStorageFull input)
        {
            _selector.IsStorageFull = input.IsFull;
        }
        
        private void EventBus_Ability_Add(AbilitiesEvents.Add input)
        {
            if (_isGameplayActive)
            {
                _selector.DecreaseValue(input.AbilityType);
            }
        }
        
        private void EventBus_Ability_SetActive(AbilitiesEvents.NotifyIsActive input)
        {
            if (_isGameplayActive == false) return;
            
            if (input.IsActive)
            {
                _selector.IncreaseValue(input.AbilityType);
                _isTimerRunning = false;
                RemoveTimer(_spawnTimerId);
            }
            else
            {
                TryRunTimer(spawnDelay);
            }
        }

        #endregion

        #region Projectile

        private void EventBus_Projectile_Spawn(ProjectileEvents.Spawn input)
        {
            if (input.ProjectileType == ProjectileType.AbilitySphere)
            {
                CreateAbilitySphere(new ProjectileSpawnData
                {
                    Position = input.Position,
                    Direction = input.Direction,
                    MovementMultiplier = input.MovementMultiplier
                });
            }
        }
        
        private void EventBus_Projectile_DisableSpawn(ProjectileEvents.DisableSpawn input)
        {
            _isGameplayActive = false;
            RemoveTimer(_spawnTimerId);
            _selector.Reset();
            _spawner.RecycleAll();
        }
        
        private void EventBus_Projectile_EnableSpawn(ProjectileEvents.EnableSpawn input)
        {
            _isGameplayActive = true;
        }
        
        private void EventBus_Projectile_UpdateLevel(ProjectileEvents.UpdateLevel input)
        {
            _currentLevel = input.Level;
            _selector.UpdateLevel(_currentLevel);
            if (_currentLevel >= _minUnlockLevel &&
                _hasTimerEnable == false)
            {
                _hasTimerEnable = true;
                TryRunTimer(spawnDelay);
            }
        }

        #endregion

        #endregion
    }
}