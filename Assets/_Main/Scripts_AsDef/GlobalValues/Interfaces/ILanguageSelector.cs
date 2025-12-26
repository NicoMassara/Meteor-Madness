using System;

namespace MeteorMadness.GlobalValues.Interfaces
{
    public interface ILanguageSelector
    {
        public event Action<int> OnChanged;
    }
}