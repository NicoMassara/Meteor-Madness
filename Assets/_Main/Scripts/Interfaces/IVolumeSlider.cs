using System;

namespace _Main.Scripts.Interfaces
{
    public interface IVolumeSlider
    {
        public event Action<float> OnChanged;
    }
}