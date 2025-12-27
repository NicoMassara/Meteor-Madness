using System;

namespace MeteorMadness.Contracts.Interfaces
{
    public interface ILanguageSelector
    {
        public event Action<int> OnChanged;
    }
}