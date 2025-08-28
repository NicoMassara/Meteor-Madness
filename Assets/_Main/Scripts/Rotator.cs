using UnityEngine;

namespace _Main.Scripts
{
    public class Rotator
    {
        private float _rotationSpeed;
        private readonly Transform _transform;

        public Rotator(Transform transform, float rotationSpeed = 100)
        {
            _transform = transform;
            _rotationSpeed = rotationSpeed;
        }

        public void Rotate()
        {
            _transform.Rotate(0,0,_rotationSpeed * Time.deltaTime);
        }

        public void SetSpeed(float newSpeed)
        {
            _rotationSpeed = newSpeed;
        }
    }
}