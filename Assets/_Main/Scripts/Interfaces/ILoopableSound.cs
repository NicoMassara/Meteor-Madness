using System;

namespace _Main.Scripts.Interfaces
{
    public interface ILoopableSound
    {
        public event Action OnLoopFinished;
    }
}