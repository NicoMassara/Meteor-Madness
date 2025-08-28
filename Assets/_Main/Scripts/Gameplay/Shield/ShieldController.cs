using System;
using _Main.Scripts.Sounds;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Shield
{
    [RequireComponent(typeof(ShieldMotor))]
    public class ShieldController : MonoBehaviour
    {
        private ShieldMotor _motor;

        private void Awake()
        {
            _motor = GetComponent<ShieldMotor>();
        }

        public void SetActiveSprite(bool isActive)
        {
            _motor.SetActiveSprite(isActive);
        }

        public void HitShield()
        {
            _motor.HitShield();
        }

        public void ShrinkShield()
        {
            _motor.ShrinkShield();
        }

        public void Rotate(int direction)
        {
            _motor.Rotate(direction);
        }

        public void Restart()
        {
            _motor.RestartPosition();
        }
    }
}