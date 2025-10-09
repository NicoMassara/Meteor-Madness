using System;

namespace _Main.Scripts.Interfaces
{
    public interface IPoolable<T>
    {
        public event Action<T> OnRecycle;
        
        public void Recycle();
    }
}