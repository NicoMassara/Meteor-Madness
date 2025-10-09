using System;
using System.Collections.Generic;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Gameplay.Abilities.Sphere;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyTools;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Spawn
{
    public class AbilitySpawner : ManagedBehavior
    {
        [Header("Components")]
        [SerializeField] private AbilitySphereView prefab;
        [Header("Values")] 
        [Range(5, 15f)] 
        [SerializeField] private float spawnDelay = 5f;

        private bool _isGameplayActive;
        private bool _isStorageFull;
        private bool _isTimerRunning;
        private ulong _spawnTimerId;
        private int _minUnlockLevel;
        private int _currentLevel;
        private AbilitySphereFactory _factory;
        private AbilitySelector _selector;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Awake()
        {
            SetEventBus();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var tempDic = new Dictionary<AbilityType, int>();
                
                for (int i = 0; i < 10; i++)
                {
                    var ability = GetAbilityToAdd();
                    if (tempDic.ContainsKey(ability))
                    {
                        tempDic[ability]++;
                    }
                    else
                    {
                        tempDic.Add(ability, 1);
                    }
                }

                foreach (var item in tempDic)
                {
                    Debug.Log($"Ability: {item.Key}, Times Selected: {item.Value}");
                }
            }
        }

        private void Start()
        {
            var selectorData = GameConfigManager.Instance.GetGameplayData().AbilitySelectorData;
            _minUnlockLevel = selectorData.MinUnlockLevel;
            
            _selector = new AbilitySelector(selectorData.GetRarityValues,selectorData.GetUnlockLevelValues);
            _factory = new AbilitySphereFactory(prefab);
        }
        
        private void SendAbility()
        {
            AbilitiesEventCaller.RequestSpawn();
        }

        private void CreateAbilitySphere(Vector2 position, Vector2 direction, float movementMultiplier)
        {
            var tempSphere = _factory.SpawnAbilitySphere();
            var movementSpeed = GameConfigManager.Instance.GetGameplayData().ProjectileData.MaxProjectileSpeed 
                                * movementMultiplier;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            tempSphere.SetValues(new AbilitySphereValues
            {
                MovementSpeed = movementSpeed,
                Rotation = tempRot,
                Position = position,
                Direction = direction.normalized,
                AbilityType = GetAbilityToAdd()
            });
            tempSphere.OnDeflection += DeflectionHandler;
            tempSphere.OnEarthCollision += OnEarthCollisionHandler;
            
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
        }

        private void SetTimer(float time)
        {
            Debug.Log($"Timer Set To: {time}");

            _isTimerRunning = true;
            _spawnTimerId = TimerManager.Add(new TimerData
            {
                Time = time,
                OnEndAction = () =>
                {
                    SendAbility();
                    _isTimerRunning = false;
                }
            }, SelfUpdateGroup);
        }

        private void RemoveTimer()
        {
            TimerManager.Remove(_spawnTimerId);
        }

        private void TryRunTimer(float time)
        {
            if(GetCanRunTimer() == false) return;
            
            SetTimer(time);
        }
        
        private AbilityType GetAbilityToAdd()
        {
            return _selector.GetAbilityToAdd();
        }

        private bool GetCanRunTimer()
        {
            return _isGameplayActive && 
                   _currentLevel >= _minUnlockLevel && 
                   _isTimerRunning == false;
        }

        #region EventBus

        private void SetEventBus()
        {
            GameEventCaller.Subscribe<ProjectileEvents.Spawn>(EventBus_Projectile_Spawn);
            GameEventCaller.Subscribe<AbilitiesEvents.SetStorageFull>(EventBus_Ability_StorageFull);
            GameEventCaller.Subscribe<AbilitiesEvents.SetActive>(EventBus_Ability_SetActive);
            GameEventCaller.Subscribe<AbilitiesEvents.Add>(EventBus_Ability_Add);
            GameEventCaller.Subscribe<GameModeEvents.Finish>(EventBus_GameMode_Finished);
            GameEventCaller.Subscribe<GameModeEvents.Start>(EventBus_GameMode_Start);
            GameEventCaller.Subscribe<GameModeEvents.Disable>(EventBus_GameMode_Disable);
            GameEventCaller.Subscribe<GameModeEvents.UpdateLevel>(EventBus_GameMode_UpdateLevel);
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
        
        private void EventBus_Ability_SetActive(AbilitiesEvents.SetActive input)
        {
            if (_isGameplayActive == false) return;
            
            if (input.IsActive)
            {
                _selector.IncreaseValue(input.AbilityType);
                RemoveTimer();
            }
            else
            {
                TryRunTimer(spawnDelay);
            }
        }

        private void EventBus_GameMode_Start(GameModeEvents.Start input)
        {
            _isGameplayActive = true;
        }

        private void EventBus_Projectile_Spawn(ProjectileEvents.Spawn input)
        {
            if (input.ProjectileType == ProjectileType.AbilitySphere)
            {
                CreateAbilitySphere(input.Position, input.Direction, input.MovementMultiplier);
            }
        }
        
        private void EventBus_GameMode_UpdateLevel(GameModeEvents.UpdateLevel input)
        {
            _currentLevel = input.CurrentLevel;
            _selector.UpdateLevel(_currentLevel);
            if (GetCanRunTimer())
            {
                TryRunTimer(spawnDelay);
            }
        }

        private void EventBus_GameMode_Disable(GameModeEvents.Disable input)
        {
            _isGameplayActive = false;
            TimerManager.Remove(_spawnTimerId);
            _factory.RecycleAll();
        }

        private void EventBus_GameMode_Finished(GameModeEvents.Finish input)
        {
            RemoveTimer();
            _selector.Reset();
            _factory.RecycleAll();
        }

        #endregion
    }

    public class AbilitySelector
    {
        public bool IsStorageFull { get; set; }

        private readonly Roulette _roulette = new Roulette();
        private readonly Func<Tuple<AbilityType[], int[]>> _getValuesAction;
        private readonly Func<Tuple<int[],AbilityType[]>> _getUnlockAction;
        private readonly Dictionary<AbilityType, ActionValue> _multipliers = new Dictionary<AbilityType, ActionValue>();

        private class ActionValue
        {
            public readonly AbilityType AbilityType;
            public int Value { get; private set; }

            public ActionValue(AbilityType abilityType)
            {
                AbilityType = abilityType;
            }

            public void IncreaseValue()
            {
                Value += 5;
            }

            public void DecreaseValue()
            {
                Value -= 5;
            }

            public void EnableValue()
            {
                Value = 10;
            }

            public void DisableValue()
            {
                Value = 0;
            }
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
        
        public void Reset()
        {
            ResetMultipliers();
        }

        public AbilityType GetAbilityToAdd()
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
            
            return _roulette.Run(tempDic);
        }
    }
}


























