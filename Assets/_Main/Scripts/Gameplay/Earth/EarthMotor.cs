using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthMotor : ObservableComponent
    {
        private float _currentHealth;
        private float _deathShakeTimer;

        public void Execute()
        {
            if (_deathShakeTimer > 0)
            {
                _deathShakeTimer -= Time.deltaTime;

                if (_deathShakeTimer <= 0)
                {
                    NotifyAll(EarthObserverMessage.SetActiveDeathShake, false);
                }
            }
        }

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
                NotifyAll(EarthObserverMessage.DeclareDeath);
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
            _deathShakeTimer = 3f;
            NotifyAll(EarthObserverMessage.SetActiveDeathShake, isShaking);
        }
    }

    public enum EarthSpriteType
    {
        Normal,
        Broken
    }
}