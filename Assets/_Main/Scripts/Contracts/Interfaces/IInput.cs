using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IInput
    {
        public void Enable();
        public void Disable();
        
        public event Action<int> OnUpdateDirection;
        public event Action<bool> OnTriggerAbility;
        public event Action OnPaused;
    }
}