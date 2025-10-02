using System;

namespace _Main.Scripts.MyTools
{
    public interface IPoolable<T>
    {
        public event Action<T> OnRecycle;
        
        public void Recycle();
    }
}