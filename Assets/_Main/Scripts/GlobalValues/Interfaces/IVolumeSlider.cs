using System;

namespace MeteorMadness.GlobalValues.Interfaces
{
    public interface IVolumeSlider
    {
        public event Action<float> OnChanged;
    }
}