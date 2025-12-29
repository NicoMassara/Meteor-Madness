using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface IButtonSelector
    {
        public event Action<int> OnSkinSelected;
    }
}