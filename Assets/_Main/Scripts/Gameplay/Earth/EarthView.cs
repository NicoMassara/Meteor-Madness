using System;
using _Main.Scripts.Shaker;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject spriteObject;
        [SerializeField] private GameObject brokenSpriteObject;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Header("Values")]
        [SerializeField] private AnimationCurve shakeMultiplier;
        [SerializeField] private ShakeDataSo shakeData;
        
        private ShakerController _shakerController;
        
        private EarthMotor _motor;
        private float _currentHealth;
        private float _targetHealth;
        private float _elapsedTime;
        private float _duration = 0.5f;
        private float _shakeTime;
        private bool _isDead;

        private void Awake()
        {
            _motor = GetComponent<EarthMotor>();
            
            spriteObject.SetActive(true);
            brokenSpriteObject.SetActive(false);
        }

        private void Start()
        {
            _currentHealth = 1;
            _targetHealth = _currentHealth;
            
            _shakerController = new ShakerController(spriteObject.transform);
            _shakerController.SetShakeData(shakeData);
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(_targetHealth));
            
            _motor.OnDamage += OnDamagedHandler;
            _motor.OnHeal += OnHealHandler;
            _motor.OnDeath += OnDeathHandler;
            _motor.OnRestart += OnRestartHandler;
            _motor.OnDestruction += OnDestructionHandler;
        }

        private void Update()
        {
            if (_isDead == false)
            {
                _shakerController.HandleShake();
            }
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
            _isDead = true;
            _shakerController.SetMultiplier(shakeMultiplier.Evaluate(_targetHealth));
            UpdateColor();
        }

        private void OnDestructionHandler()
        {
            spriteObject.SetActive(false);
            brokenSpriteObject.SetActive(true);
        }

        private void OnRestartHandler()
        {
            spriteObject.SetActive(true);
            brokenSpriteObject.SetActive(false);
            _targetHealth = 1;
            _isDead = false;
            UpdateColor();
        }
        #endregion
        
    }
}