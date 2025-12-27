using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IVibrationToggle
    {
        public event Action<bool> OnChanged;
    }
}