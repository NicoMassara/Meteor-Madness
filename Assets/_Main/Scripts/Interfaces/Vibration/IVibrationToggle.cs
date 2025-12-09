using System;

namespace _Main.Scripts.Interfaces
{
    public interface IVibrationToggle
    {
        public event Action<bool> OnChanged;
    }
}