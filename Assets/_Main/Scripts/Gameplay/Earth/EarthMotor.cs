using _Main.Scripts.Observer;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthMotor : ObservableComponent
    {
        private float _currentHealth;

        public void RestartHealth()
        {
            _currentHealth = 1;
            NotifyAll(EarthObserverMessage.RestartHealth);
        }

        public void MakeDamage(float damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                NotifyAll(EarthObserverMessage.DeclareDeath);
            }
            else
            {
                NotifyAll(EarthObserverMessage.MakeDamage, _currentHealth);
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
            NotifyAll(EarthObserverMessage.SetActiveDeathShake, isShaking);
        }
    }

    public enum EarthSpriteType
    {
        Normal,
        Broken
    }
}