using System;

namespace MeteorMadness.GlobalValues.Interfaces
{
    public interface IPoolable<T>
    {
        public event Action<T> OnRecycle;
        
        public void Recycle();
    }
}