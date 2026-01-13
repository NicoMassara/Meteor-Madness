using MeteorMadness.Core.FlyingObject.Contracts;
using MeteorMadness.GlobalValues.Tools.Observer;
using UnityEngine;

namespace MeteorMadness.Core.FlyingObject
{
    public abstract class FlyingObjectMotor<T> : ObservableComponent
    where T : FlyingObjectValues
    {
        private T _data;
        
        public virtual void SetValues(T data)
        {
            _data = data;
            NotifyAll(FlyingObjectObserverMessage.SetValues, _data);
        }

        public virtual void HandleCollision()
        {
            NotifyAll(FlyingObjectObserverMessage.HandleCollision, _data);
        }

        public virtual void UpdatePosition(Vector2 position)
        {
            if(_data == null) return;
            
            _data.Position = position;
            NotifyAll(FlyingObjectObserverMessage.UpdatePosition, _data.Position);
        }
    }
}