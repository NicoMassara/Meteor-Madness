using System;

namespace _Main.Scripts.Interfaces
{
    public interface IInputReader
    {
        public bool HasUsedAbility { get; }
        public event Action<int> OnMovementDirectionChanged;
        public event Action OnStopMovement;
        public event Action<bool>  OnAbilityTriggered;
    }
}