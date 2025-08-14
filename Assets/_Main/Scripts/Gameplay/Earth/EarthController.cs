using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Range(0, 30)] [SerializeField] private float startHealDelay = 15f;
        [Range(0, 15)] [SerializeField] private float keepHealDelay = 2f;

        private float _currentHealth;
        private float _lastHealth;
        private float _startHealTimer;
        private float _keepHealTimer;
        
        public UnityAction OnDeath;
        
        private void Start()
        {
            _currentHealth = 1;
        }

        private void Update()
        {
            if (_currentHealth < 1)
            {
                _startHealTimer -= Time.deltaTime;
                if (_startHealTimer <= 0)
                {
                    _keepHealTimer -= Time.deltaTime;
                    if (_keepHealTimer <= 0)
                    {
                        Heal();
                        _keepHealTimer = keepHealDelay;
                    }
                }
                UpdateColor();
            }
        }
        
        public void Restart()
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            _currentHealth = 1;
            _lastHealth = _currentHealth;
        }

        public void Damage()
        {
            _lastHealth = _currentHealth;
            _currentHealth -= GameValues.BrutalMeteorDamage;
            _startHealTimer = startHealDelay;
            _keepHealTimer = 0;
            
            UpdateColor();

            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        private void Heal()
        {
            _lastHealth = _currentHealth;
            _currentHealth += 0.1f;
            UpdateColor();
        }

        private void UpdateColor()
        {
            _lastHealth = Mathf.Lerp(_lastHealth,_currentHealth, 10*Time.deltaTime);
            var tempColor = new Color(1, _lastHealth, _lastHealth);
            spriteRenderer.color = tempColor;
        }
    }
}