using System;
using _Main.Scripts.Shaker;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private GameObject completeSpriteObject;
        [SerializeField] private GameObject brokenSpriteObject;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Header("Values")]
        [SerializeField] private AnimationCurve shakeMultiplier;
        [SerializeField] private ShakeDataSo healthShakeData;
        [SerializeField] private ShakeDataSo deathShakeData;
        [Header("Rotation")] 
        [Range(0, 100)] [SerializeField] private float rotationSpeed = 25;
        
        private ShakerController _shakerController;
        private Rotator _rotator;
        
        private EarthMotor _motor;
        private float _currentHealth;
        private float _targetHealth;
        private bool _canRotate;

        private readonly Timer _startRotationTimer = new Timer();

        private void Awake()
        {
            _motor = GetComponent<EarthMotor>();
            
            completeSpriteObject.SetActive(true);
            brokenSpriteObject.SetActive(false);
        }

        private void Start()
        {
            _currentHealth = 1;
            _targetHealth = _currentHealth;
            
            _rotator = new Rotator(spriteContainer.transform, rotationSpeed);
            _startRotationTimer.OnEnd += StartRotationTimerOnEndHandler;
            
            _shakerController = new ShakerController(spriteContainer.transform);
            _shakerController.SetShakeData(healthShakeData);
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(_targetHealth));
            
            _motor.OnDamage += OnDamagedHandler;
            _motor.OnHeal += OnHealHandler;
            _motor.OnDeath += OnDeathHandler;
            _motor.OnRestart += OnRestartHandler;
            _motor.OnDestruction += OnDestructionHandler;
            _motor.OnShake += OnShakeHandler;
        }
        

        private void Update()
        {
            _shakerController.HandleShake();

            if (_canRotate == true)
            {
                _rotator.Rotate();
            }
            
            _startRotationTimer.Run();
        }

        private void UpdateColor()
        {
            spriteRenderer.color = new Color(1, _targetHealth, _targetHealth);
        }

        #region Handlers
        
        private void OnDamagedHandler(float healthAmount)
        {
            _targetHealth = healthAmount;
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(_targetHealth));
            _rotator.SetSpeed(rotationSpeed * _targetHealth);
            UpdateColor();
        }
        
        private void OnHealHandler(float healthAmount)
        {
            _targetHealth = healthAmount;
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(_targetHealth));
            UpdateColor();
        }
        
        private void OnDeathHandler()
        {
            _targetHealth = 0;
            _canRotate = false;
            _shakerController.SetMultiplier(0);
            UpdateColor();
        }

        private void OnDestructionHandler()
        {
            completeSpriteObject.SetActive(false);
            brokenSpriteObject.SetActive(true);
            _startRotationTimer.Set(GameTimeValues.StartRotatingAfterDeath);
        }

        private void OnRestartHandler()
        {
            completeSpriteObject.SetActive(true);
            brokenSpriteObject.SetActive(false);
            _targetHealth = 1;
            _canRotate = true;
            _shakerController.SetShakeData(healthShakeData);
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(_targetHealth));
            spriteContainer.transform.localRotation = Quaternion.Euler(0, 0, 0);
            UpdateColor();
        }
        
        private void OnShakeHandler(bool isShaking)
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
        
        private void StartRotationTimerOnEndHandler()
        {
            _rotator.SetSpeed(rotationSpeed/2);
            _canRotate = true;
        }
        
        #endregion
        
    }
}