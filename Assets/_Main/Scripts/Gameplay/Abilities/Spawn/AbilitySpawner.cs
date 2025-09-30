﻿using System;
using _Main.Scripts.FyingObject;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Gameplay.Abilities.Sphere;
using _Main.Scripts.Gameplay.FlyingObject;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Spawn
{
    public class AbilitySpawner : ManagedBehavior, IUpdatable
    {
        [Header("Components")]
        [SerializeField] private ProjectileSpawnLocationController spawnLocation;
        [SerializeField] private AbilitySphereView prefab;
        [SerializeField] private ProjectileSpawnDataSo spawanData;
        [Header("Values")] 
        [Range(5, 15f)] 
        [SerializeField] private float spawnDelay = 5f;
        [Space]
        [Header("Testing")] 
        [SerializeField] private bool doesSpawn;
        
        private AbilitySphereFactory _factory;
        private ProjectileSpawnValues _spawnValues;
        private ulong _spawnTimerId;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Awake()
        {
            SetEventBus();
        }

        private void Start()
        {
            _factory = new AbilitySphereFactory(prefab);
            _spawnValues = new ProjectileSpawnValues(spawanData);
        }

        public void ManagedUpdate() { }
        
        private void SendAbility()
        {
            var temp = _factory.SpawnAbilitySphere();
            var spawnPosition = spawnLocation.GetPositionByAngle(spawnLocation.GetSpawnAngle(), spawnLocation.GetSpawnRadius());
            var movementSpeed = (GameValues.MaxMeteorSpeed/2) * _spawnValues.GetMovementMultiplier();
            
            Vector2 direction = spawnLocation.GetCenterOfGravity() - spawnPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            temp.SetValues(new AbilitySphereValues
            {
                MovementSpeed = movementSpeed,
                Rotation = tempRot,
                Position = spawnPosition,
                Direction = direction.normalized,
                AbilityType = GetAbilityToAdd()
            });
            temp.OnDeflection += DeflectionHandler;
            temp.OnEarthCollision += OnEarthCollisionHandler;
            
            SetTimer();
        }

        private void DeflectionHandler(AbilitySphereCollisionData data)
        {
            data.Sphere.OnDeflection = null;
            data.Sphere.OnEarthCollision = null;
            
            GameManager.Instance.EventManager.Publish(new AbilitiesEvents.Add{AbilityType = data.Ability});
            GameManager.Instance.EventManager.Publish
            (
                new MeteorEvents.Deflected
                {
                    Position = data.Position,
                    Rotation = data.Rotation,
                    Direction = data.Direction
                }
            );
            
            data.Sphere.ForceRecycle();
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
                    Direction = data.Direction
                }
            );
            
            data.Sphere.ForceRecycle();
        }

        private void SetTimer()
        {
            if(doesSpawn == false) return;
            
            _spawnTimerId = TimerManager.Add(new TimerData
            {
                Time = spawnDelay,
                OnEndAction = SendAbility
            }, SelfUpdateGroup);
        }

        private void RemoveTimer()
        {
            TimerManager.Remove(_spawnTimerId);
        }

        private AbilityType GetAbilityToAdd()
        {
            return (AbilityType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AbilityType)).Length-1);
        }

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<GameModeEvents.Finish>(EventBus_OnGameFinished);
            eventManager.Subscribe<GameModeEvents.Start>(EventBus_OnGameStart);
            eventManager.Subscribe<AbilitiesEvents.EnableSpawner>(EventBus_OnAbilityInUse);
            eventManager.Subscribe<GameModeEvents.UpdateLevel>(EnventBus_OnUpdateLevel);
            eventManager.Subscribe<GameModeEvents.Disable>(EventBus_OnGameModeDisable);
        }

        private void EventBus_OnGameModeDisable(GameModeEvents.Disable input)
        {
            TimerManager.Remove(_spawnTimerId);
            _factory.RecycleAll();
        }

        private void EnventBus_OnUpdateLevel(GameModeEvents.UpdateLevel input)
        {
            _spawnValues.SetIndex(input.CurrentLevel);
        }

        private void EventBus_OnGameStart(GameModeEvents.Start input)
        {
            SetTimer();
        }

        private void EventBus_OnAbilityInUse(AbilitiesEvents.EnableSpawner input)
        {
            if (input.IsEnable)
            {
                RemoveTimer();
            }
            else
            {
                SetTimer();
            }
        }

        private void EventBus_OnGameFinished(GameModeEvents.Finish input)
        {
            TimerManager.Remove(_spawnTimerId);
            _factory.RecycleAll();
        }

        #endregion
    }
}