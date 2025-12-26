using System;

namespace MeteorMadness.GlobalValues.Interfaces.Skins
{
    
    public interface IEarthSkin
    {
        public event Action<float> OnHealthChanged;
        public event Action OnShow;
        public event Action OnHide;
    }

    public interface IFlyingObjectSkin
    {
        public event Action OnSkinEnable;
    }
}