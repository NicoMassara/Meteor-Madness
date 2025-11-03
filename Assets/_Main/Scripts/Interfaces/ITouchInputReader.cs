using System;

namespace _Main.Scripts.Interfaces
{
    public interface ITouchInputReader
    {
        public void Enable();
        public void Disable();
        
        public event Action<int> OnUpdateDirection;
        public event Action<bool> OnTriggerAbility;
    }
}