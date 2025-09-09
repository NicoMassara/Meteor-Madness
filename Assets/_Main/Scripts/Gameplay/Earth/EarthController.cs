using System;
using _Main.Scripts.Gameplay.Earth;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    [RequireComponent(typeof(EarthView))]
    public class EarthController : MonoBehaviour
    {
        private EarthMotor _motor;
        private EarthView _view;

        private void Awake()
        {
            _view = GetComponent<EarthView>();
            
            _motor = new EarthMotor();
            _motor.Subscribe(_view);
        }

        #region Health

        public void RestartHealth()
        {
            _motor.RestartHealth();
        }

        public void MakeDamage(float damage)
        {
            _motor.MakeDamage(damage);
        }

        public void Heal(float healAmount)
        {
            _motor.Heal(healAmount);
        }

        #endregion

        #region Death

        public void TriggerDestruction()
        {
            _motor.TriggerDestruction();
        }

        public void SetDeathShake(bool isShaking)
        {
            _motor.SetDeathShake(isShaking);
        }

        #endregion

        public void AddObserverToMotor(IObserver observer)
        {
            _motor.Subscribe(observer);
        }

    }
}