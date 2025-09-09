using _Main.Scripts.Observer;
using _Main.Scripts.Shaker;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthView : MonoBehaviour, IObserver
    {
        [Header("Sprite Components")]
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private GameObject normalSpriteObject;
        [SerializeField] private GameObject brokenSpriteObject;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Space]
        [Header("Sounds")]
        [SerializeField] private SoundBehavior hitSound;
        [Space]
        [Header("Shake Values")]
        [SerializeField] private AnimationCurve shakeMultiplier;
        [SerializeField] private ShakeDataSo healthShakeData;
        [SerializeField] private ShakeDataSo deathShakeData;
        [Space]
        [Header("Values")] 
        [Range(0, 100)] 
        [SerializeField] private float rotationSpeed = 25;
        
        private ShakerController _shakerController;
        private Rotator _rotator;
        private readonly Timer _rotateAfterDeathTimer = new Timer();
        private GameObject _currentSprite;
        private bool _canRotate;

        private void Awake()
        {
            
        }

        private void Start()
        {
            _rotator = new Rotator(spriteContainer.transform, rotationSpeed);
            _shakerController = new ShakerController(spriteContainer.transform);
            _shakerController.SetShakeData(healthShakeData);
            SetShakeMultiplier(1);
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
                case EarthObserverMessage.MakeDamage:
                    HandleMakeDamage((float)args[0]);
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

        private void HandleMakeDamage(float healthAmount)
        {
            hitSound?.PlaySound();
            SetShakeMultiplier(healthAmount);
            UpdateColorByHealth(healthAmount);
            _rotator.SetSpeed(rotationSpeed * healthAmount);
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
            _shakerController.SetShakeData(healthShakeData);
        }

        #endregion

        #region Death

        private void SetDeathShake(bool isShaking)
        {
            if (isShaking)
            {
                _shakerController.SetMultiplier(1);
                _shakerController.SetShakeData(deathShakeData);
            }
            else
            {
                _shakerController.SetMultiplier(0);
            }
        }

        private void HandleDestruction()
        {
            SetSpriteType(EarthSpriteType.Broken);
            _rotateAfterDeathTimer.Set(GameTimeValues.StartRotatingAfterDeath);
            _rotateAfterDeathTimer.OnEnd += RotateAfterDeathTimer_OnEndHandler;
        }

        private void HandleDeath()
        {
            _canRotate = false;
            _shakerController.SetMultiplier(0);
            UpdateColorByHealth(0);
        }

        #endregion

        #region Sprite

        private void SetSpriteType(EarthSpriteType spriteType)
        {
            _currentSprite?.SetActive(false);

            switch (spriteType)
            {
                case EarthSpriteType.Normal:
                    _currentSprite = normalSpriteObject;
                    break;
                case EarthSpriteType.Broken:
                    _currentSprite = brokenSpriteObject;
                    break;
            }
            
            _currentSprite?.SetActive(true);
        }

        #endregion

        private void SetShakeMultiplier(float currentHealth)
        {
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(currentHealth));
        }

        private void UpdateColorByHealth(float currentHealth)
        {
            spriteRenderer.color = new Color(1, currentHealth, currentHealth);
        }
        
        private void RotateAfterDeathTimer_OnEndHandler()
        {
            _rotateAfterDeathTimer.OnEnd -= RotateAfterDeathTimer_OnEndHandler;
            _rotator.SetSpeed(rotationSpeed/2);
            _canRotate = true;
        }
    }
}