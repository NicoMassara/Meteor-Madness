using System;

namespace _Main.Scripts.Interfaces
{
    public interface ILanguageSelector
    {
        public event Action<int> OnChanged;
    }
}