using System;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthMotor : ObservableComponent
    {
        private float _currentHealth = 1;
        private bool _canRotate;
        private bool _isShaking;
        private bool _canTakeDamage = true;
        private const float LowHealthThreshold = 0.35f;
        public event Action OnDeath;
        

        public void RestartHealth()
        {
            NotifyAll(EarthObserverMessage.RestartHealth,_currentHealth);
            _currentHealth = 1;
            NotifyAll(EarthObserverMessage.Debug_UpdateHealth, _currentHealth);
        }

        public void HandleCollision(float damage, Vector3 position, Quaternion rotation, Vector2 direction)
        {
            if(_canTakeDamage == false) return;
            
            _currentHealth -= damage;

            if (_currentHealth < 0)
            {
                _currentHealth = 0;
            }

            NotifyAll(EarthObserverMessage.SetLowHealth, GetIsLowHealth());
            NotifyAll(EarthObserverMessage.EarthCollision, _currentHealth, position, rotation,direction);
            NotifyAll(EarthObserverMessage.Debug_UpdateHealth, _currentHealth);
            
            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float heal)
        {
            var lastAmount = _currentHealth;
            _currentHealth += heal;
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, 1f);
            NotifyAll(EarthObserverMessage.Heal, _currentHealth,lastAmount);
            NotifyAll(EarthObserverMessage.Debug_UpdateHealth, _currentHealth);
        }

        public void TriggerDestruction()
        {
            NotifyAll(EarthObserverMessage.TriggerDestruction);
        }

        public void SetDeathShake(bool isShaking)
        {
            _isShaking = isShaking;
            NotifyAll(EarthObserverMessage.SetActiveDeathShake, _isShaking);
        }

        public void SetRotation(bool canRotate)
        {
        }

        public void TriggerDeath()
        {
            NotifyAll(EarthObserverMessage.DeclareDeath);
        }

        public void TriggerEndDestruction()
        {
            NotifyAll(EarthObserverMessage.TriggerEndDestruction);
        }

        public void SetEnableDamage(bool canTakeDamage)
        {
            _canTakeDamage = canTakeDamage;
        }

        private bool GetIsLowHealth()
        {
            return _currentHealth <= LowHealthThreshold;
        }
    }
}