using System;
using _Main.Scripts.Gameplay.Earth;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthController 
    {
        private readonly EarthMotor _motor;

        public EarthController(EarthMotor motor)
        {
            _motor = motor;
        }

        #region Health

        public void RestartHealth()
        {
            _motor.RestartHealth();
        }

        public void HandleCollision(float damage, Vector3 position, Quaternion rotation)
        {
            _motor.HandleCollision(damage, position, rotation);
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
    }
}