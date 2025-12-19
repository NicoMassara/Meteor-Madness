using System;

namespace _Main.Scripts.Interfaces
{
    public interface IInputReader
    {
        public event Action<int> OnMovementDirectionChanged;
        public event Action<bool>  OnAbilityTriggered;
    }
}