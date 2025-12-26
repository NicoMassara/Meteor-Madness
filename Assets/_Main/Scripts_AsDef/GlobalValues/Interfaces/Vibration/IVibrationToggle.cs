using System;

namespace MeteorMadness.GlobalValues.Interfaces
{
    public interface IVibrationToggle
    {
        public event Action<bool> OnChanged;
    }
}