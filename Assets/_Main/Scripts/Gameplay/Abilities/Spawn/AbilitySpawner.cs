using System;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Gameplay.Abilities.Sphere;
using _Main.Scripts.Gameplay.FlyingObject.Projectile;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Spawn
{
    public class AbilitySpawner : ManagedBehavior, IUpdatable
    {
        [Header("Components")]
        [SerializeField] private ProjectileSpawnSettings spawnSettings;
        [SerializeField] private AbilitySphereView prefab;
        [Header("Values")] 
        [Range(5, 15f)] 
        [SerializeField] private float spawnDelay = 5f;
        
        private AbilitySphereFactory _factory;
        private ulong _spawnTimerId;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Awake()
        {
            SetEventBus();
        }

        private void Start()
        {
            _factory = new AbilitySphereFactory(prefab);
        }

        public void ManagedUpdate() { }
        
        private void SendAbility()
        {
            var temp = _factory.SpawnAbilitySphere();
            var spawnPosition = spawnSettings.GetPositionByAngle(spawnSettings.GetSpawnAngle(), spawnSettings.GetSpawnRadius());
            var movementSpeed = (GameValues.MaxMeteorSpeed) * spawnSettings.GetMovementMultiplier();
            
            Vector2 direction = spawnSettings.GetCenterOfGravity() - spawnPosition;
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
            
            GameManager.Instance.EventManager.Publish(new ProjectileEvents.Add{Projectile = temp});
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

            var temp = UnityEngine.Random.Range(spawnDelay, spawnDelay * 1.15f);
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
                    Direction = data.Direction
                }
            );
            
            data.Sphere.ForceRecycle();
            
            var temp = UnityEngine.Random.Range(spawnDelay * 0.75f, spawnDelay );
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
            return (AbilityType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AbilityType)).Length-1);
        }

        #region EventBus

        private void SetEventBus()
        {
            var eventManager = GameManager.Instance.EventManager;
            eventManager.Subscribe<GameModeEvents.Finish>(EventBus_OnGameFinished);
            eventManager.Subscribe<AbilitiesEvents.EnableSpawner>(EventBus_OnAbilityInUse);
            eventManager.Subscribe<GameModeEvents.Disable>(EventBus_OnGameModeDisable);
            eventManager.Subscribe<GameModeEvents.UpdateLevel>(EventBus_GameMode_UpdateLevel);
        }

        private void EventBus_GameMode_UpdateLevel(GameModeEvents.UpdateLevel input)
        {
            if (input.CurrentLevel == GameValues.LevelToSpawnAbilities)
            {
                Debug.Log("Abilities Spawning");
                SetTimer(spawnDelay);
            }
        }

        private void EventBus_OnGameModeDisable(GameModeEvents.Disable input)
        {
            TimerManager.Remove(_spawnTimerId);
            _factory.RecycleAll();
        }

        private void EventBus_OnAbilityInUse(AbilitiesEvents.EnableSpawner input)
        {
            if (input.IsEnable)
            {
                RemoveTimer();
            }
            else
            {
                SetTimer(spawnDelay);
            }
        }

        private void EventBus_OnGameFinished(GameModeEvents.Finish input)
        {
            RemoveTimer();
            _factory.RecycleAll();
        }

        #endregion
    }
}