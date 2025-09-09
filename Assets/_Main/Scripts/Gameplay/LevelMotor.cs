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

namespace _Main.Scripts.Gameplay
{
    public class LevelMotor : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private MeteorSpawner meteorSpawner;
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
            
            meteorSpawner.OnShieldHit += Meteor_OnShieldHitHandler;
            meteorSpawner.OnEarthHit += Meteor_OnEarthHitHandler;
            earthController.OnDeath += Earth_OnDeathHandler;
            earthController.OnDamage += Earth_OnDamageHandler;
            earthController.OnDestruction += Earth_OnDestructionHandler;
        }
        

        public void StartLevel()
        {
            _meteorHitCount = 0;
            _meteorSaveCount = 0;
            earthController.Restart();
            shieldController.RestartPosition();
            shieldController.TransitionToActive();
            deathTheme.StopSound();
            gameplayTheme.PlaySound();
            cameraController.ZoomOut();
            
            OnStart?.Invoke();
        }
        

        public void StartEndLevel()
        {            
            gameplayTheme.StopSound();
            shieldController.TransitionToUnactive();
            meteorSpawner.RecycleAll();
        }

        public void FinishEndLevel()
        {
            deathTheme.PlaySound();
            OnEnd?.Invoke(_meteorSaveCount);
        }

        public void ZoomIn()
        {
            cameraController.ZoomIn();
        }

        public void RestartLevel()
        {
            _meteorSpeedController.RestartAll();
        }

        public void TriggerDestruction()
        {
            earthController.TriggerDestruction();
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
            var meteorSpeed = GameValues.BaseMeteorSpeed * _meteorSpeedController.GetCurrentMultiplier();
            meteorSpawner.SpawnSingleMeteor(meteorSpeed);
        }

        #endregion
        
        #region Handlers

        private void Meteor_OnEarthHitHandler(Vector3 position, Quaternion rotation)
        {
            _meteorHitCount++;
            cameraController.StartShake(earthHitShake);
            var direction = (centerOfGravity.position - position).normalized;
            particlesController.SpawnParticle(collisionSprite, position, rotation, direction);
            _meteorSpeedController.RestartCount();
            OnEarthHit?.Invoke(_meteorHitCount);
            earthController.Damage();
        }

        private void Meteor_OnShieldHitHandler(Vector3 position)
        {
            _meteorSaveCount++;
            cameraController.StartShake(shieldHitShake);
            var direction = (centerOfGravity.position - position).normalized;
            particlesController.SpawnParticle(shieldHitSprite, position, quaternion.identity, direction);
            _meteorSpeedController.CheckForNextLevel(_meteorSaveCount);
            shieldController.Hit();
            OnShieldHit?.Invoke(_meteorSaveCount);
        }
        
        private void Earth_OnDeathHandler()
        {
            OnEnd?.Invoke(_meteorSaveCount);
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