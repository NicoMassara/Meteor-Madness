using UnityEngine;

namespace _Main.Scripts.FyingObject
{
    public abstract class FlyingObjectController<T> where T : FlyingObjectMotor
    {
        protected T Motor { get; private set; }

        protected FlyingObjectController(T motor)
        {
            Motor = motor;
        }

        public virtual void SetValues(float movementSpeed, Quaternion rotation, Vector2 position, Vector2 direction)
        {
            Motor.SetValues(movementSpeed, rotation, position, direction);
        }

        public abstract void HandleTriggerEnter2D(Collider2D other);

        public void UpdatePosition(Vector2 transformPosition)
        {
            Motor.UpdatePosition(transformPosition);
        }
    }
}