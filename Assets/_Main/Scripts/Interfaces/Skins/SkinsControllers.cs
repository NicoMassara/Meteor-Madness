using System;

namespace _Main.Scripts.Interfaces
{
    
    public interface IEarthSkin
    {
        public event Action<float> OnHealthChanged;
    }

    public interface IFlyingObjectSkin
    {
        public event Action OnSkinEnable;
    }
}