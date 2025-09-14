using UnityEngine;

namespace _Main.Scripts
{
    public class Rotator
    {
        private float _rotationSpeed;
        private Vector3 _rotationDirection;
        private readonly Transform[] _transforms;

        public Rotator(Transform transform, Vector3 direction, float rotationSpeed = 100)
        {
            _transforms = new [] { transform };
            _rotationSpeed = rotationSpeed;
            _rotationDirection = direction;
        }
        
        public Rotator(Transform[] transforms, Vector3 direction, float rotationSpeed = 100)
        {
            _transforms = transforms;
            _rotationSpeed = rotationSpeed;
            _rotationDirection = direction;
        }

        public void Rotate(float deltaTime)
        {
            foreach (Transform transform in _transforms)
            {
                transform.Rotate(_rotationDirection, _rotationSpeed * deltaTime);
            }
        }

        public void SetSpeed(float newSpeed)
        {
            _rotationSpeed = newSpeed;
        }
    }
}