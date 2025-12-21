using System;
using System.Collections.Generic;
using _Main.Scripts.Abilities.So;
using _Main.Scripts.Abilities.Sphere;
using _Main.Scripts.Managers;
using _Main.Scripts.GameConfig;
using _Main.Scripts.MyTools;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;
using _Main.Scripts.GlobalEvents;

namespace _Main.Scripts.Abilities.Spawn
{
    public class AbilitySpawner : ManagedBehavior
    {
        #region Private Classes

        public class AbilitySelector
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
        
        [Header("Components")]
        [SerializeField] private AbilitySphereView prefab;
        [SerializeField] private AbilitySelectorDataSo selectorData;
        [Header("Values")] 
        [Range(5, 15f)] 
        [SerializeField] private float spawnDelay = 5f;
        
        private bool _hasTimerEnable;
        private bool _isGameplayActive;
        private bool _isStorageFull;
        private bool _isTimerRunning;
        private int _minUnlockLevel;
        private int _currentLevel;
        private TimerManager.GeneratedId _spawnTimerId;
        private AbilitySphereFactory _factory;
        private AbilitySelector _selector;
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private AbilitySpawnDebugData _debugData;
#endif
        
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
            _factory = new AbilitySphereFactory(prefab);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData = new AbilitySpawnDebugData
            {
                MinSpawnLevel = _minUnlockLevel
            };
#endif
            
            
            BootEvents.SubSystemInitialized();
        }
        private void SendAbility()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.HasSentAbility = true;
#endif
            AbilitiesEventCaller.RequestSpawn();
        }
        private void CreateAbilitySphere(Vector2 position, Vector2 direction, float movementMultiplier)
        {
            var tempSphere = _factory.SpawnAbilitySphere();
            var movementSpeed = GameConfigManager.Instance.GetGameplayData().ProjectileData.MaxProjectileSpeed 
                                * movementMultiplier;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            var ability = GetAbilityToAdd();
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.LastAbilitySpawned = ability;
#endif
            
            tempSphere.SetValues(new AbilitySphereValues
            {
                MovementSpeed = movementSpeed,
                Rotation = tempRot,
                Position = position,
                Direction = direction.normalized,
                AbilityType = ability
            });
            tempSphere.OnDeflection += DeflectionHandler;
            tempSphere.OnEarthCollision += OnEarthCollisionHandler;
            tempSphere.EnableMovement = true;
            
            Debug.LogWarning(ability);
            
            ProjectileEventCaller.Add(tempSphere);
        }
        private void DeflectionHandler(AbilitySphereCollisionData data)
        {
            data.Sphere.OnDeflection = null;
            data.Sphere.OnEarthCollision = null;
            
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
            
            data.Sphere.Recycle();

            var temp = UnityEngine.Random.Range(spawnDelay, spawnDelay * 1.15f);
            temp = _isStorageFull ? temp/2 : temp;
            TryRunTimer(temp);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.HasSentAbility = false;
#endif
        }
        private void OnEarthCollisionHandler(AbilitySphereCollisionData data)
        {
            data.Sphere.OnDeflection = null;
            data.Sphere.OnEarthCollision = null;
            
            
            ProjectileEventCaller.Collision(new CollisionData
            {
                Position = data.Position,
                Rotation = data.Rotation,
                Direction = data.Direction,
                Type = ProjectileType.AbilitySphere
            });
            
            data.Sphere.Recycle();
            
            var temp = UnityEngine.Random.Range(spawnDelay * 0.75f, spawnDelay);
            temp = _isStorageFull ? temp/2 : temp;
            TryRunTimer(temp);
            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.HasSentAbility = false;
#endif
        }
        private void SetTimer(float time)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            _debugData.IsRunningSpawnTimer = true;
            _debugData.NextSpawnDelay = time;
#endif
            
            _isTimerRunning = true;
            _spawnTimerId = TimerManager.Add(new TimerData(time,() =>
            {
                SendAbility();
                _isTimerRunning = false;
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                _debugData.NextSpawnDelay = 0;
                _debugData.IsRunningSpawnTimer = false;
#endif
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
                CreateAbilitySphere(input.Position, input.Direction, input.MovementMultiplier);
            }
        }
        
        private void EventBus_Projectile_DisableSpawn(ProjectileEvents.DisableSpawn input)
        {
            _isGameplayActive = false;
            RemoveTimer(_spawnTimerId);
            _selector.Reset();
            _factory.RecycleAll();
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


























