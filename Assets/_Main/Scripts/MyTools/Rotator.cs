using UnityEngine;

namespace _Main.Scripts
{
    public class Rotator
    {
        private float _rotationSpeed;
        private readonly Transform[] _transforms;

        public Rotator(Transform transform, float rotationSpeed = 100)
        {
            _transforms = new [] { transform };
            _rotationSpeed = rotationSpeed;
        }
        
        public Rotator(Transform[] transforms, float rotationSpeed = 100)
        {
            _transforms = transforms;
            _rotationSpeed = rotationSpeed;
        }

        public void Rotate(float deltaTime)
        {
            foreach (Transform transform in _transforms)
            {
                transform.Rotate(Vector3.forward, _rotationSpeed * deltaTime);
            }
        }

        public void SetSpeed(float newSpeed)
        {
            _rotationSpeed = newSpeed;
        }
    }
}