using System;

namespace _Main.Scripts.Interfaces
{
    public interface IButtonSelector
    {
        public event Action<int> OnSkinSelected;
    }
}