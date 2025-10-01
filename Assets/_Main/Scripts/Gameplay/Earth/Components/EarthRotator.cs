using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthRotator
    {
        private Rotator _modelRotator;
        private Rotator _spriteRotator;

        public EarthRotator(Transform modelTransform, Transform spriteTransform, float rotationSpeed)
        {
            _modelRotator = new Rotator(modelTransform, Vector3.down, rotationSpeed);
            _spriteRotator =  new Rotator(spriteTransform, Vector3.forward, rotationSpeed);
        }

        public void Rotate(float deltaTime, bool isDead)
        {
            if (isDead)
            {
                _spriteRotator.Rotate(deltaTime);
            }
            else
            {
                _modelRotator.Rotate(deltaTime);
            }
        }

        public void SetRotationSpeed(float speed)
        {
            _spriteRotator.SetSpeed(speed);
            _modelRotator.SetSpeed(speed);
        }
    }
}