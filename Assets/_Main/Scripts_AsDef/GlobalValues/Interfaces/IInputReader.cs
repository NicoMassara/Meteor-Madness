using System;

namespace MeteorMadness.GlobalValues.Interfaces
{
    public interface IInputReader
    {
        public event Action<int> OnMovementDirectionChanged;
        public event Action<bool>  OnAbilityTriggered;
    }
}