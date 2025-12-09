using System;

namespace _Main.Scripts.GlobalEvents
{
    public class LocalizationEvents
    {
        public static event Action OnLanguageChanged;
        public static event Action OnLocalizationLoaded;

        public static void TriggerOnLanguageChanged()
        {
            OnLanguageChanged?.Invoke();
        }

        public static void TriggerOnLocalizationLoaded()
        {
            OnLocalizationLoaded?.Invoke();
        }
    }
}