using System;

namespace MeteorMadness.GlobalValues.Interfaces
{
    public interface IButtonSelector
    {
        public event Action<int> OnSkinSelected;
    }
}