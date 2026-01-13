using MeteorMadness.Core.FlyingObject.Contracts;
using UnityEngine;

namespace MeteorMadness.Core.FlyingObject
{
    public abstract class FlyingObjectController<T>
        where T : FlyingObjectValues
    {
        protected readonly FlyingObjectMotor<T> Motor;

        public FlyingObjectController(FlyingObjectMotor<T> motor)
        {
            Motor = motor;
        }
        
        public void SetValues(T data) => Motor.SetValues(data);
        public void UpdatePosition(Vector2 position) => Motor.UpdatePosition(position);
    }   
}