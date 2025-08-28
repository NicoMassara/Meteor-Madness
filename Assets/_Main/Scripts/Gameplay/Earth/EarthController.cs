using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthController : MonoBehaviour
    {
        private EarthMotor _motor;
        
        public UnityAction OnDeath;
        public UnityAction OnDamage;
        public UnityAction OnDestruction;

        private void Awake()
        {
            _motor = GetComponent<EarthMotor>();
        }

        private void Start()
        {
            _motor.OnDeath += OnDeathHandler;
            _motor.OnDestruction += OnDestructionHandler;
        }



        private void Update()
        {
            if (_motor.IsHealing())
            {
                //_motor.RunHealTimer();
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

        public void TriggerDestruction()
        {
            _motor.TriggerDestruction();
        }

        #region Handlers

        private void OnDeathHandler()
        {
            OnDeath?.Invoke();
        }
        
        private void OnDestructionHandler()
        {
            OnDestruction?.Invoke();
        }

        #endregion

        public void StartShake()
        {
            _motor.StartShake();
        }

        public void StopShake()
        {
            _motor.StopShake();
        }
    }
}