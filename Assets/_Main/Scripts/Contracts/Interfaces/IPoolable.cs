using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IPoolable<T>
    {
        public event Action<T> OnRecycle;
        
        public void Recycle();
    }
}