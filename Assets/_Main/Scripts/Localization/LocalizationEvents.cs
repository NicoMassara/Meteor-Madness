using System;

namespace _Main.Scripts.Localization
{
    public class LocalizationEvents
    {
        public static event Action OnLanguageChanged;

        public static void TriggerOnLanguageChanged()
        {
            OnLanguageChanged?.Invoke();
        }
    }
}