using System;
using _Main.Scripts.Gameplay.Abilies;
using _Main.Scripts.Gameplay.Abilities.Sphere;
using _Main.Scripts.Gameplay.Meteor;
using _Main.Scripts.Managers;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Abilities.Spawn
{
    [RequireComponent(typeof(MeteorLocationSpawnController))]
    public class AbilitySpawner : ManagedBehavior, IUpdatable
    {
        [Header("Components")]
        [SerializeField] private AbilitySphereView prefab;
        [SerializeField] private Transform centerOfGravity;
        [Range(5, 15f)] 
        [SerializeField] private float spawnDelay = 5f;
        [Header("Values")] 
        [Range(22f,100f)]
        [SerializeField] private float spawnRadius;
        
        private MeteorLocationSpawnController _locationSpawn;
        private AbilitySphereFactory _factory;
        private ulong _spawnTimerId;
        public UpdateGroup SelfUpdateGroup { get; } = UpdateGroup.Gameplay;

        private void Awake()
        {
            _locationSpawn = GetComponent<MeteorLocationSpawnController>();
        }

        private void Start()
        {
            _factory = new AbilitySphereFactory(prefab);
            
            SetEventBus();
            SetTimer();
        }

        public void ManagedUpdate() { }
        
        private void SendAbility()
        {
            var temp = _factory.SpawnAbilitySphere();
            var spawnPosition = _locationSpawn.GetPositionByAngle(_locationSpawn.GetSpawnAngle(), spawnRadius);
            var movementSpeed = 10f;

            Debug.Log(spawnPosition);
                
            //Set Direction and Rotation towards COG
            Vector2 direction = (Vector2)centerOfGravity.position - spawnPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var tempRot = Quaternion.AngleAxis(angle, Vector3.forward);
            temp.SetSphereValues(new AbilitySphereValues
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
            
            GameManager.Instance.EventManager.Publish(new AddAbility{AbilityType = data.Ability});
            
            data.Sphere.ForceRecycle();
        }
        
        private void OnEarthCollisionHandler(AbilitySphereCollisionData data)
        {
            data.Sphere.OnDeflection = null;
            data.Sphere.OnEarthCollision = null;
            
            GameManager.Instance.EventManager.Publish
            (
                new MeteorCollision
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
            _spawnTimerId = TimerManager.Add(new TimerData
            {
                Time = spawnDelay,
                OnEndAction = SendAbility
            }, SelfUpdateGroup);
        }

        private AbilityType GetAbilityToAdd()
        {
            return (AbilityType)UnityEngine.Random.Range(1, Enum.GetValues(typeof(AbilityType)).Length-1);
        }

        #region EventBus

        private void SetEventBus()
        {
            GameManager.Instance.EventManager.Subscribe<GameFinished>(EventBus_OnGameFinished);
        }

        private void EventBus_OnGameFinished(GameFinished input)
        {
            TimerManager.Remove(_spawnTimerId);
            _factory.RecycleAll();
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius);
        }
    }
}