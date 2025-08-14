using _Main.Scripts.Gameplay.Earth;
using _Main.Scripts.Gameplay.Meteor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay
{
    public class LevelMotor : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MeteorFactory meteorFactory;
        [SerializeField] private EarthController earthController;
        [SerializeField] private Transform centerOfGravity;
        [Header("Values")]
        [SerializeField] private float spawnRadius;
        [Range(0.1f, 5)]
        [SerializeField] private float spawnDelay;
        [Range(5, 50)] 
        [SerializeField] private float baseMeteorSpeed = 15f;
        
        private MultiplierController _multiplierController;
        private int _meteorSaveCount;
        private int _meteorHitCount;
        private float _spawnTimer;

        public UnityAction<int> OnShieldHit;
        public UnityAction<int> OnEarthHit;
        public UnityAction<int> OnDeath;
        public UnityAction OnStart;

        private void Awake()
        {
            _multiplierController = new MultiplierController();
        }

        private void Start()
        {
            meteorFactory.OnShieldHit += OnShieldHitHandler;
            meteorFactory.OnEarthHit += OnEarthHitHandler;
            earthController.OnDeath += OnDeathHandler;
            
            _spawnTimer = spawnDelay;
        }

        public void RunSpawnTimer()
        {
            _spawnTimer -= Time.deltaTime;
        }

        public bool HasSpawnTimerEnd()
        {
            return _spawnTimer <= 0;
        }

        public void SpawnMeteor()
        {
            Vector2 spawnPosition = GetRandomPointInRadiusRange(centerOfGravity.position, 
                spawnRadius*0.75f, spawnRadius*1.25f);
            var meteorSpeed = baseMeteorSpeed * _multiplierController.GetCurrentMultiplier();
            var finalSpeed = Random.Range(meteorSpeed*0.85f, meteorSpeed*1.15f);
            meteorFactory.SpawnMeteor(finalSpeed,spawnPosition);
        }

        public void RestartSpawnTimer()
        {
            var finalTimer = spawnDelay / _multiplierController.GetCurrentMultiplier();
            _spawnTimer = Random.Range(finalTimer*0.85f, finalTimer*1.15f);
        }

        public void RestartLevel()
        {
            _meteorHitCount = 0;
            _meteorSaveCount = 0;
            _multiplierController.Restart();
            earthController.Restart();
        }

        private Vector2 GetRandomPointInRadiusRange(Vector2 center, float minRadius, float maxRadius)
        {
            // Random angle in radians
            float angle = Random.Range(0f, Mathf.PI * 2f);

            // Random distance between min and max radius
            float distance = Random.Range(minRadius, maxRadius);

            // Convert polar coordinates to Cartesian
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            return center + offset;
        }

        #region Handlers

        private void OnEarthHitHandler()
        {
            _meteorHitCount++;
            OnEarthHit?.Invoke(_meteorHitCount);
            earthController.Damage();
        }

        private void OnShieldHitHandler()
        {
            _meteorSaveCount++;
            _multiplierController.CheckForNextLevel(_meteorSaveCount);
            OnShieldHit?.Invoke(_meteorSaveCount);
        }
        
        private void OnDeathHandler()
        {
            OnDeath?.Invoke(_meteorSaveCount);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius*0.75f);
            Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius*1.25f);
        }
    }
}