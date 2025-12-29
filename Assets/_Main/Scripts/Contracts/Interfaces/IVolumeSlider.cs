using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IVolumeSlider
    {
        public event Action<float> OnChanged;
    }
}