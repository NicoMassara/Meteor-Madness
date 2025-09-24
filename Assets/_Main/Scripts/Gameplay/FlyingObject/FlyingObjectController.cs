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

        public virtual void SetValues(FlyingObjectValues data)
        {
            Motor.SetValues(data);
        }

        public abstract void HandleTriggerEnter2D(Collider2D other);

        public void UpdatePosition(Vector2 transformPosition)
        {
            Motor.UpdatePosition(transformPosition);
        }
    }
}