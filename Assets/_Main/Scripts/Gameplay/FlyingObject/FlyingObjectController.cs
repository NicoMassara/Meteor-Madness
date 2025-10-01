using _Main.Scripts.Comet;
using UnityEngine;

namespace _Main.Scripts.FyingObject
{
    public abstract class FlyingObjectController<T, TVS> 
        where T : FlyingObjectMotor<TVS> 
        where TVS : FlyingObjectValues
    {
        protected T Motor { get; private set; }

        protected FlyingObjectController(T motor)
        {
            Motor = motor;
        }

        public virtual void SetValues(TVS data)
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