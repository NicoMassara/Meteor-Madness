using _Main.Scripts.Sounds;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthMotor : MonoBehaviour
    {
        [Header("Sounds")]
        [SerializeField] private SoundBehavior hitSound;
        [Header("Values")]
        [Range(0, 30)] [SerializeField] private float startHealDelay = 15f;
        [Range(0, 15)] [SerializeField] private float keepHealDelay = 2f;
        [SerializeField] private AnimationCurve healthRatioCurve;
        
        private float _currentHealth;
        private float _startHealTimer;
        private float _keepHealTimer;
        
        public UnityAction OnDeath;
        public UnityAction<float> OnDamage;
        public UnityAction<float> OnHeal;
        public UnityAction OnRestart;
        public UnityAction<bool> OnShake;
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
            _currentHealth -= GameValues.HardMeteorDamage;
            _startHealTimer = startHealDelay;
            _keepHealTimer = 0;
            
            hitSound.PlaySound(1f, healthRatioCurve.Evaluate(_currentHealth));
            

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

        public void StartShake()
        {
            OnShake?.Invoke(true);
        }

        public void StopShake()
        {
            OnShake?.Invoke(false);
        }
    }
}