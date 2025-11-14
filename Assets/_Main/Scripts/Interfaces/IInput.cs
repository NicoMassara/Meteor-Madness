using System;

namespace _Main.Scripts.Interfaces
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