using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthController : MonoBehaviour
    {
        private EarthMotor _motor;
        
        public UnityAction OnDeath;

        private void Awake()
        {
            _motor = GetComponent<EarthMotor>();
        }

        private void Start()
        {
            _motor.OnDeath += OnDeathHandler;
        }

        private void Update()
        {
            if (_motor.IsHealing())
            {
                _motor.RunHealTimer();
            }
        }
        
        public void Restart()
        {
            _motor.Restart();
        }

        public void Damage()
        {
            _motor.Damage();
        }

        #region Handlers

        private void OnDeathHandler()
        {
            OnDeath?.Invoke();
        }

        #endregion
    }
}