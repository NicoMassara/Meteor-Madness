using System;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthMotor : ObservableComponent
    {
        private float _currentHealth;
        private bool _canRotate;
        private bool _isShaking;
        public event Action OnDeath;
        

        public void RestartHealth()
        {
            _currentHealth = 1;
            NotifyAll(EarthObserverMessage.RestartHealth);
        }

        public void HandleCollision(float damage, Vector3 position, Quaternion rotation)
        {
            _currentHealth -= damage;
            
            NotifyAll(EarthObserverMessage.EarthCollision, _currentHealth, position, rotation);
            
            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float heal)
        {
            _currentHealth += heal;
            NotifyAll(EarthObserverMessage.Heal);
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
            _canRotate = canRotate;
            NotifyAll(EarthObserverMessage.SetRotation, _canRotate);
        }

        public void TriggerDeath()
        {
            NotifyAll(EarthObserverMessage.DeclareDeath);
        }

        public void TriggerEndDestruction()
        {
            NotifyAll(EarthObserverMessage.TriggerEndDestruction);
        }
    }

    public enum EarthSpriteType
    {
        Normal,
        Broken
    }
}