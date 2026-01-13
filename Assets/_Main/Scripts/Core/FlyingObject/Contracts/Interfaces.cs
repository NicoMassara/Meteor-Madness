using System;

namespace MeteorMadness.Core.FlyingObject.Contracts
{
    public interface IFlyingObject
    {
        public event Action OnObjectEnabled;
        public event Action OnObjectDisabled;
    }

    public interface IDebugFlyingObject
    {
        public float Speed { get; }
        public float Position { get; }
    }

    public interface IFlyingObjectWithValues<T> 
        where T : FlyingObjectValues
    {
        public void SetValues(T data);
    }
} 