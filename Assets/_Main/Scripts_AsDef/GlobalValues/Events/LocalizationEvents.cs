using System;

namespace MeteorMadness.GlobalValues.Events
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