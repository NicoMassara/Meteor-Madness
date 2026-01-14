using System;
using UnityEngine;

namespace MeteorMadness.Core.FlyingObject.Contracts
{
    public interface IFlyingObject<T> 
        where T : FlyingObjectValues
    {
        public event Action OnObjectEnabled;
        public event Action OnObjectDisabled;
        public void SetValues(T data);
        public Vector2 Position { get; }
        public void SetEnableMovement(bool enable);
        public void Recycle();
    }

    public interface IDebugFlyingObject
    {
        public float Speed { get; }
        public Vector2 Position { get; }
    }
    
    public interface IFlyingObjectView<T>
        where T : FlyingObjectValues
    {
        public event Action<Vector2> OnPositionChanged;
        public event Action<T> OnValuesSet;
    }

    public interface IFlyingObjectController<T>
        where T : FlyingObjectValues
    {
        public void SetValues(T data);
        public void UpdatePosition(Vector2 position);
    }
} 