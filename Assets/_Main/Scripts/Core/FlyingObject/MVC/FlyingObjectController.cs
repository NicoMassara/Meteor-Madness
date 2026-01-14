using MeteorMadness.Core.FlyingObject.Contracts;
using UnityEngine;

namespace MeteorMadness.Core.FlyingObject
{
    public abstract class FlyingObjectController<T,TS> : IFlyingObjectController<T>
        where T : FlyingObjectValues
        where TS : FlyingObjectMotor<T>
    {
        protected readonly TS Motor;

        public FlyingObjectController(TS motor)
        {
            Motor = motor;
        }
        
        public void SetValues(T data) => Motor.SetValues(data);
        public void UpdatePosition(Vector2 position) => Motor.UpdatePosition(position);
    }   
}