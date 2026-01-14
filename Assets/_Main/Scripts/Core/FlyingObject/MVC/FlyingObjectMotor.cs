using MeteorMadness.Core.FlyingObject.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.Core.FlyingObject
{
    public abstract class FlyingObjectMotor<T> : ObservableComponent
    where T : FlyingObjectValues
    {
        protected T Data { get; private set; }

        public virtual void SetValues(T data)
        {
            Data = data;
            NotifyAll(FlyingObjectObserverMessage.SetValues, Data);
        }

        public virtual void HandleCollision()
        {
            NotifyAll(FlyingObjectObserverMessage.HandleCollision, Data);
        }

        public virtual void UpdatePosition(Vector2 position)
        {
            if(Data == null) return;
            
            Data.Position = position;
            NotifyAll(FlyingObjectObserverMessage.UpdatePosition, Data.Position);
        }
    }
}