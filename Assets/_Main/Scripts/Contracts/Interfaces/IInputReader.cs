using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IInputReader
    {
        public event Action<int> OnMovementDirectionChanged;
        public event Action<bool>  OnAbilityTriggered;
    }
}