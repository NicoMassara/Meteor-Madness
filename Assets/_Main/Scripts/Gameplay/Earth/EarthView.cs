using _Main.Scripts.Managers;
using _Main.Scripts.Observer;
using _Main.Scripts.Particles;
using _Main.Scripts.Shaker;
using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthView : MonoBehaviour, IObserver
    {
        [Header("Sprite Components")]
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private GameObject normalSpriteObject;
        [SerializeField] private GameObject brokenSpriteObject;
        [SerializeField] private SpriteRenderer normalSpriteRenderer;
        [Space]
        [Header("Sounds")]
        [SerializeField] private SoundBehavior collisionSound;
        [SerializeField] private SoundBehavior deathSound;
        [Space]
        [Header("Shake Values")]
        [SerializeField] private AnimationCurve shakeMultiplier;
        [SerializeField] private ShakeDataSo healthShakeData;
        [SerializeField] private ShakeDataSo deathShakeData;
        [SerializeField] private ShakeDataSo cameraShakeData;
        [Space]
        [Header("Values")] 
        [Range(0, 100)] 
        [SerializeField] private float rotationSpeed = 25;
        [SerializeField] private ParticleDataSo collisionParticleData;
        
        private EarthController _controller;
        private ShakerController _shakerController;
        private Rotator _rotator;
        private readonly Timer _rotateAfterDeathTimer = new Timer();
        private GameObject _currentSprite;
        private bool _canRotate;
        

        private void Awake()
        {
            _rotator = new Rotator(spriteContainer.transform, rotationSpeed);
            _shakerController = new ShakerController(spriteContainer.transform);
        }

        private void Start()
        {
            _shakerController.SetShakeData(healthShakeData);
            SetShakeMultiplier(1);
            SetSpriteType(EarthSpriteType.Normal);
        }

        private void Update()
        {
            _shakerController.HandleShake();
            
            if (_canRotate == true)
            {
                _rotator.Rotate();
            }
            
            _rotateAfterDeathTimer.Run();
        }

        public void OnNotify(string message, params object[] args)
        {
            switch (message)
            {
                case EarthObserverMessage.RestartHealth:
                    HandleRestartHealth();
                    break;
                case EarthObserverMessage.EarthCollision:
                    HandleCollision((float)args[0],(Vector3)args[1],(Quaternion)args[2]);
                    break;
                case EarthObserverMessage.DeclareDeath:
                    HandleDeath();
                    break;
                case EarthObserverMessage.TriggerDestruction:
                    HandleDestruction();
                    break;
                case EarthObserverMessage.SetActiveDeathShake:
                    SetDeathShake((bool)args[0]);
                    break;
                case EarthObserverMessage.Heal:
                    HandleHeal((float)args[0]);
                    break;
                case EarthObserverMessage.SetSprite:
                    SetSpriteType((EarthSpriteType)args[0]);
                    break;
            }
        }
        
        #region Health
        
        private void HandleCollision(float healthAmount, Vector3 position, Quaternion rotation)
        {
            collisionSound?.PlaySound();
            SetShakeMultiplier(healthAmount);
            UpdateColorByHealth(healthAmount);
            _rotator.SetSpeed(rotationSpeed * healthAmount);
            
            GameManager.Instance.EventManager.Publish
            (
                new SpawnParticle
                {
                    ParticleData = collisionParticleData,
                    Position = position,
                    Rotation = rotation
                }
            );
            
            GameManager.Instance.EventManager.Publish(new CameraShake{ShakeData = cameraShakeData});
        }
        
        private void HandleHeal(float healthAmount)
        {
            SetShakeMultiplier(healthAmount);
            UpdateColorByHealth(healthAmount);
        }
        private void HandleRestartHealth()
        {
            UpdateColorByHealth(1);
            SetSpriteType(EarthSpriteType.Normal);
            SetShakeMultiplier(1);
            _canRotate = true;
            _shakerController.SetShakeData(healthShakeData);
        }

        #endregion

        #region Death

        // ReSharper disable Unity.PerformanceAnalysis
        private void SetDeathShake(bool isShaking)
        {
            if (isShaking)
            {
                _shakerController.SetMultiplier(1);
                _shakerController.SetShakeData(deathShakeData);
                GameManager.Instance.EventManager.Publish(new EarthShake());
            }
            else
            {
                _shakerController.SetMultiplier(0);
                _controller.TriggerDestruction();
            }
        }

        private void HandleDestruction()
        {
            deathSound?.PlaySound();
            SetSpriteType(EarthSpriteType.Broken);
            _rotateAfterDeathTimer.Set(GameTimeValues.StartRotatingAfterDeath);
            _rotateAfterDeathTimer.OnEnd += RotateAfterDeathTimer_OnEndHandler;
            GameManager.Instance.EventManager.Publish(new EarthDestruction());
        }

        private void HandleDeath()
        {
            _canRotate = false;
            _shakerController.SetMultiplier(0);
            UpdateColorByHealth(0);
            GameManager.Instance.EventManager.Publish(new EarthDeath());
            _controller.SetDeathShake(true);
        }

        #endregion

        #region Sprite

        private void SetSpriteType(EarthSpriteType spriteType)
        {
            _currentSprite?.SetActive(false);

            _currentSprite = spriteType switch
            {
                EarthSpriteType.Normal => normalSpriteObject,
                EarthSpriteType.Broken => brokenSpriteObject,
                _ => _currentSprite
            };

            _currentSprite?.SetActive(true);
        }

        #endregion

        private void SetShakeMultiplier(float currentHealth)
        {
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(currentHealth));
        }

        private void UpdateColorByHealth(float currentHealth)
        {
            normalSpriteRenderer.color = new Color(1, currentHealth, currentHealth);
        }
        
        private void RotateAfterDeathTimer_OnEndHandler()
        {
            _rotateAfterDeathTimer.OnEnd -= RotateAfterDeathTimer_OnEndHandler;
            _rotator.SetSpeed(rotationSpeed/2);
            _canRotate = true;
        }

        public void SetController(EarthController controller)
        {
            _controller = controller;
        }
    }
}