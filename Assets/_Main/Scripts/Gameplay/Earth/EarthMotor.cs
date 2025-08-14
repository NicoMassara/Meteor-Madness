using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthMotor : MonoBehaviour
    {
        [Header("Values")]
        [Range(0, 30)] [SerializeField] private float startHealDelay = 15f;
        [Range(0, 15)] [SerializeField] private float keepHealDelay = 2f;
        
        private float _currentHealth;
        private float _startHealTimer;
        private float _keepHealTimer;
        
        public UnityAction OnDeath;
        public UnityAction<float> OnDamage;
        public UnityAction<float> OnHeal;
        public UnityAction OnRestart;
        public UnityAction OnDestruction;
        
        private void Start()
        {
            _currentHealth = 1;
        }

        public bool IsHealing()
        {
            return _currentHealth < 1;
        }

        public void RunHealTimer()
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
            }
        }

        public void Restart()
        {
            OnRestart?.Invoke();
            _currentHealth = 1;
        }

        public void Damage()
        {
            _currentHealth -= GameValues.StandardMeteorDamage;
            _startHealTimer = startHealDelay;
            _keepHealTimer = 0;
            
            
            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
            else
            {
                OnDamage?.Invoke(_currentHealth);
            }
        }

        private void Heal()
        {
            _currentHealth += 0.1f;
            OnHeal?.Invoke(_currentHealth);
        }

        public void TriggerDestruction()
        {
            OnDestruction?.Invoke();
        }
    }
}