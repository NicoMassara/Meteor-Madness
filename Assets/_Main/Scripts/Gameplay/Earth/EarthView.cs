using System;
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
        [Range(0.1f, 100)] 
        [SerializeField] private float shakeIntensity = 20;
        [Range(0,.1f)]
        [SerializeField] private float shakeMagnitude = 0;
        
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
            _motor.OnDamage += OnDamagedHandler;
            _motor.OnHeal += OnHealHandler;
            _motor.OnDeath += OnDeathHandler;
            _motor.OnRestart += OnRestartHandler;
            _motor.OnDestruction += OnDestructionHandler;

            _currentHealth = 1;
            _targetHealth = _currentHealth;
        }

        private void Update()
        {
            if (_isDead == false)
            {
                if (_targetHealth < 1)
                {
                    HandleShake();
                }
            }
        }

        private void UpdateColor()
        {
            spriteRenderer.color = new Color(1, _targetHealth, _targetHealth);
        }

        private void HandleShake()
        {
            _shakeTime += Time.deltaTime;
            spriteObject.transform.localPosition = GetShakeOffset(_shakeTime);
        }
        
        public Vector3 GetShakeOffset(float time)
        {
            float multiplier = shakeMultiplier.Evaluate(_targetHealth);
            float angle = ((shakeIntensity * multiplier) * Mathf.PI * 2f) * time;
            float offsetX = Mathf.Sin(angle) * (shakeMagnitude * multiplier);
            float offsetY = Mathf.Cos(angle) * (shakeMagnitude * multiplier);
            return new Vector3(offsetX, offsetY, 0f);
        }

        private void Test()
        {
            if (_elapsedTime < _duration)
            {
                _elapsedTime += Time.deltaTime;
                float t = _elapsedTime / _duration;
                _currentHealth = Mathf.Lerp(_currentHealth, _targetHealth, t);
            }
        }

        #region Handlers
        
        private void OnDamagedHandler(float healthAmount)
        {
            _targetHealth = healthAmount;
            UpdateColor();
        }
        
        private void OnHealHandler(float healthAmount)
        {
            _targetHealth = healthAmount;
            UpdateColor();
        }
        
        private void OnDeathHandler()
        {
            _targetHealth = 0;
            _isDead = true;
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