using _Main.Scripts.Gameplay.Earth;
using _Main.Scripts.Gameplay.Meteor;
using _Main.Scripts.Gameplay.Shield;
using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using _Main.Scripts.Sounds;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Main.Scripts.Gameplay
{
    public class LevelMotor : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MeteorFactory meteorFactory;
        [SerializeField] private EarthController earthController;
        [SerializeField] private ShieldController shieldController;
        [SerializeField] private Transform centerOfGravity;
        [Header("Values")]
        [SerializeField] private float spawnRadius;
        [Header("Particles")]
        [SerializeField] private ParticlesController particlesController;
        [SerializeField] private ParticleDataSo collisionSprite;
        [SerializeField] private ParticleDataSo shieldHitSprite;
        [FormerlySerializedAs("cameraShaker")]
        [Header("Camera Shake")] 
        [SerializeField] private CameraController cameraController;
        [SerializeField] private ShakeDataSo earthHitShake;
        [SerializeField] private ShakeDataSo shieldHitShake;
        [Header("Sounds")] 
        [SerializeField] private SoundBehavior gameplayTheme;
        [SerializeField] private SoundBehavior deathSound;
        [SerializeField] private SoundBehavior deathTheme;
        [SerializeField] private SoundBehavior pauseSound;
        
        private MeteorSpeedController _meteorSpeedController;
        private int _meteorSaveCount;
        private int _meteorHitCount;

        public UnityAction<int> OnShieldHit;
        public UnityAction<int> OnEarthHit;
        public UnityAction<int> OnEnd;
        public UnityAction OnStart;

        private void Start()
        {
            _meteorSpeedController = new MeteorSpeedController();
            
            meteorFactory.OnShieldHit += Meteor_OnShieldHitHandler;
            meteorFactory.OnEarthHit += Meteor_OnEarthHitHandler;
            earthController.OnDeath += Earth_OnDeathHandler;
            earthController.OnDamage += Earth_OnDamageHandler;
            earthController.OnDestruction += Earth_OnDestructionHandler;
        }
        

        public void StartLevel()
        {
            _meteorHitCount = 0;
            _meteorSaveCount = 0;
            earthController.Restart();
            shieldController.Restart();
            SetActiveShield(true);
            deathTheme.StopSound();
            gameplayTheme.PlaySound();
            cameraController.ZoomOut();
            
            OnStart?.Invoke();
        }

        public void EndLevel()
        {            
            gameplayTheme.StopSound();
            deathTheme.PlaySound();
            SetActiveShield(false);
            //particlesController.RecycleAll();
            meteorFactory.RecycleAll();
            shieldController.ShrinkShield();
            
            OnEnd?.Invoke(_meteorSaveCount);
        }

        public void RestartLevel()
        {
            _meteorSpeedController.RestartAll();
        }

        public void TriggerDestruction()
        {
            earthController.TriggerDestruction();
        }

        public void SetActiveShield(bool isActive)
        {
            shieldController.SetActiveSprite(isActive);
        }

        public void StartEarthShake()
        {
            earthController.StartShake();
        }

        public void StopEarthShake()
        {
            earthController.StopShake();
        }
        
        public void SetPaused(bool isPaused)
        {
            if (isPaused)
            {
                gameplayTheme.PauseSound();
                pauseSound.PlaySound();
            }
            else
            {
                gameplayTheme.PlaySound();
            }
        }

        #region Spawner

        public void SpawnMeteor()
        {
            Vector2 spawnPosition = GetRandomPointInRadiusRange(centerOfGravity.position, 
                spawnRadius*0.75f, spawnRadius*1.25f);
            var meteorSpeed = GameValues.BaseMeteorSpeed * _meteorSpeedController.GetCurrentMultiplier();
            var finalSpeed = Random.Range(meteorSpeed*0.85f, meteorSpeed*1.15f);
            meteorFactory.SpawnMeteor(finalSpeed,spawnPosition);
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

        #endregion
        
        #region Handlers

        private void Meteor_OnEarthHitHandler(Vector3 position, Quaternion rotation)
        {
            _meteorHitCount++;
            cameraController.StartShake(earthHitShake);
            particlesController.SpawnParticle(collisionSprite, position, rotation);
            _meteorSpeedController.RestartCount();
            OnEarthHit?.Invoke(_meteorHitCount);
            shieldController.ShrinkShield();
            earthController.Damage();
        }

        private void Meteor_OnShieldHitHandler(Vector3 position)
        {
            _meteorSaveCount++;
            cameraController.StartShake(shieldHitShake);
            particlesController.SpawnParticle(shieldHitSprite, position, quaternion.identity);
            _meteorSpeedController.CheckForNextLevel(_meteorSaveCount);
            shieldController.HitShield();
            OnShieldHit?.Invoke(_meteorSaveCount);
        }
        
        private void Earth_OnDeathHandler()
        {
            cameraController.ZoomIn();
            EndLevel();
        }
        
        private void Earth_OnDamageHandler()
        {

        }
        
        private void Earth_OnDestructionHandler()
        {
            deathSound.PlaySound();
        }

        #endregion

        private void OnDrawGizmosSelected()
        
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius*0.75f);
            Gizmos.DrawWireSphere(centerOfGravity.position, spawnRadius*1.25f);
        }
    }
}