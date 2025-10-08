﻿using System;
using System.Collections.Generic;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Gameplay.Abilities.Sphere;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using _Main.Scripts.MyTools;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Spawn
{
    public class AbilitySpawner : ManagedBehavior, IUpdatable
    {
        [Header("Components")]
        [SerializeField] private AbilitySphereView prefab;
        [Header("Values")] 
        [Range(5, 15f)] 
        [SerializeField] private float spawnDelay = 5f;

        private bool _isStorageFull;
        private AbilitySphereFactory _factory;
        private AbilitySelector _selector;
        private ulong _spawnTimerId;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Awake()
        {
            SetEventBus();
        }

        private void Start()
        {
            var selectorData = GameConfigManager.Instance.GetGameplayData().AbilitySelectorData;
            var rarityTuple = selectorData.GetRarityValues();
            var levelUnlockTuple = selectorData.GetUnlockLevelValues();
            
            _selector = new AbilitySelector(rarityTuple.abilityTypes, rarityTuple.rarityValues, 
                levelUnlockTuple.unlockLevels, levelUnlockTuple.abilityTypes);
            _factory = new AbilitySphereFactory(prefab);
        }

        public void ManagedUpdate() { }
        
        private void SendAbility()
        {
            GameManager.Instance.EventManager.Publish(
                new ProjectileEvents.RequestSpawn{ProjectileType = ProjectileType.AbilitySphere});
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
            
            GameManager.Instance.EventManager.Publish(new ProjectileEvents.Add{Projectile = tempSphere});
        }

        private void DeflectionHandler(AbilitySphereCollisionData data)
        {
            data.Sphere.OnDeflection = null;
            data.Sphere.OnEarthCollision = null;
            _selector.AddAbility(data.Ability);
            
            GameManager.Instance.EventManager.Publish(
                new AbilitiesEvents.Add{AbilityType = data.Ability, Position = data.Position});
            GameManager.Instance.EventManager.Publish
            (
                new MeteorEvents.Deflected
                {
                    Position = data.Position,
                    Rotation = data.Rotation,
                    Direction = data.Direction,
                    Type = ProjectileType.AbilitySphere
                }
            );
            
            data.Sphere.ForceRecycle();
            
            var temp = UnityEngine.Random.Range(spawnDelay, spawnDelay * 1.15f);
            temp = _isStorageFull ? temp/2 : temp;
            SetTimer(temp);
        }
        
        private void OnEarthCollisionHandler(AbilitySphereCollisionData data)
        {
            data.Sphere.OnDeflection = null;
            data.Sphere.OnEarthCollision = null;
            
            GameManager.Instance.EventManager.Publish
            (
                new MeteorEvents.Collision
                {
                    Position = data.Position,
                    Rotation = data.Rotation,
                    Direction = data.Direction,
                    Type = ProjectileType.AbilitySphere
                }
            );
            
            data.Sphere.ForceRecycle();
            
            var temp = UnityEngine.Random.Range(spawnDelay * 0.75f, spawnDelay);
            temp = _isStorageFull ? temp/2 : temp;
            SetTimer(temp);
        }

        private void SetTimer(float time)
        {
            _spawnTimerId = TimerManager.Add(new TimerData
            {
                Time = time,
                OnEndAction = SendAbility
            }, SelfUpdateGroup);
        }

        private void RemoveTimer()
        {
            TimerManager.Remove(_spawnTimerId);
        }

        private AbilityType GetAbilityToAdd()
        {
            return _selector.GetAbility();
        }

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<AbilitiesEvents.SetStorageFull>(EventBus_Ability_StorageFull);
            eventManager.Subscribe<AbilitiesEvents.SetActive>(EventBus_Ability_SetActive);
            eventManager.Subscribe<GameModeEvents.Finish>(EventBus_GameMode_Finished);
            eventManager.Subscribe<GameModeEvents.Disable>(EventBus_OnGameModeDisable);
            eventManager.Subscribe<GameModeEvents.UpdateLevel>(EventBus_GameMode_UpdateLevel);
            eventManager.Subscribe<ProjectileEvents.Spawn>(EventBus_Projectile_Spawn);
        }

        private void EventBus_Projectile_Spawn(ProjectileEvents.Spawn input)
        {
            if (input.ProjectileType == ProjectileType.AbilitySphere)
            {
                CreateAbilitySphere(input.Position, input.Direction, input.MovementMultiplier);
            }
        }

        private void EventBus_Ability_SetActive(AbilitiesEvents.SetActive input)
        {
            if (input.IsActive)
            {
                RemoveTimer();
                _selector.RemoveAbility(input.AbilityType);
            }
            else
            {
                SetTimer(spawnDelay);
            }
        }

        private void EventBus_Ability_StorageFull(AbilitiesEvents.SetStorageFull input)
        {
            _selector.IsStorageFull = input.IsFull;
        }

        private void EventBus_GameMode_UpdateLevel(GameModeEvents.UpdateLevel input)
        {
            if (input.CurrentLevel == 5)
            {
                _selector.SetLevel(input.CurrentLevel);
                SetTimer(spawnDelay);
            }
        }

        private void EventBus_OnGameModeDisable(GameModeEvents.Disable input)
        {
            TimerManager.Remove(_spawnTimerId);
            _factory.RecycleAll();
        }

        private void EventBus_GameMode_Finished(GameModeEvents.Finish input)
        {
            RemoveTimer();
            _factory.RecycleAll();
            _selector.CleanMultiplier();
        }

        #endregion
    }

    public class AbilitySelector
    {
        private int _level;
        public bool IsStorageFull { get; set; }

        private readonly Roulette _roulette = new Roulette();
        private readonly List<AbilityType> _storedAbilities = new List<AbilityType>();
        private readonly Dictionary<AbilityType, int> _dic = new Dictionary<AbilityType, int>();
        private readonly Dictionary<AbilityType, AbilityValue> _valuesDic = new Dictionary<AbilityType, AbilityValue>();
        private readonly Dictionary<int, AbilityType> _unlockDic = new Dictionary<int, AbilityType>();

        private class AbilityValue
        {
            private readonly int _selectValue;
            private float _multiplier;
            public float Multiplier => _multiplier;
            public AbilityType AbilityType { get; private set; }

            public AbilityValue(int selectValue, AbilityType abilityType)
            {
                _selectValue = selectValue;
                AbilityType = abilityType;
                _multiplier = 0;
            }

            public void SetMultiplier(float value)
            {
                _multiplier = value;
            }

            public int GetSelectValue()
            {
                return (int)Math.Round(_selectValue * _multiplier);
            }
        }
        
        public  AbilitySelector(
            AbilityType[] rarityTupleAbilityTypes, int[] rarityTupleRarityValues, 
            int[] unlockLevels, AbilityType[] unlockAbility)
        {
            _unlockDic = CreateDic(unlockLevels, unlockAbility);
            
            var abilityValues = new AbilityValue[rarityTupleAbilityTypes.Length];
            
            for (int i = 0; i < rarityTupleAbilityTypes.Length; i++)
            {
                abilityValues[i] = new AbilityValue(
                    rarityTupleRarityValues[i],
                    rarityTupleAbilityTypes[i]);
            }
            
            _valuesDic = CreateDic(rarityTupleAbilityTypes, abilityValues);
        }

        private Dictionary<T,TS> CreateDic<T, TS>(T[] keyArray, TS[] valueArray)
        {
            var dic = new Dictionary<T,TS>();
            
            for (int i = 0; i < keyArray.Length; i++)
            {
                var key = keyArray[i];
                if(dic.ContainsKey(key)) continue;
                dic.Add(key, valueArray[i]);
            }
            
            return dic;
        }

        
        public void SetLevel(int level)
        {
            if (_unlockDic.TryGetValue(level, out var value))
            {
                SetMultiplier(value, 1f);
            }
        }

        public void AddAbility(AbilityType abilityType)
        {
            _storedAbilities.Add(abilityType);

            var currMultiplier = GetMultiplier(abilityType);
            SetMultiplier(abilityType, currMultiplier * 0.5f);
        }

        public void RemoveAbility(AbilityType abilityType)
        {
            _storedAbilities.Remove(abilityType);
            var currMultiplier = GetMultiplier(abilityType);
            SetMultiplier(abilityType, currMultiplier / 0.5f);
        }

        private void SetMultiplier(AbilityType abilityType, float value = 1f)
        {
            _valuesDic[abilityType].SetMultiplier(value);
        }

        private float GetMultiplier(AbilityType abilityType)
        {
            return _valuesDic[abilityType].Multiplier;
        }

        public void CleanMultiplier()
        {
            for (int i = 1; i < (int)AbilityType.Automatic+1; i++)
            {
                SetMultiplier((AbilityType)i, 0f);
            }
        }

        public AbilityType GetAbility()
        {
            _dic.Clear();

            foreach (var ability in _valuesDic.Values)
            {
                _dic.Add(ability.AbilityType, ability.GetSelectValue());
            }

            return _roulette.Run(_dic);
        }
    }
}