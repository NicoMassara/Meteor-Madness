using UnityEngine;

namespace MeteorMadness.Managers.Localization
{
    public class LanguageSelector : MonoBehaviour
    {
        public static void ChangeLanguage(string languageCode)
        {
            if (!System.Enum.TryParse(languageCode, true, out SystemLanguage lang))
            {
                Debug.LogWarning($"Invalid language code: {languageCode}");
                return;
            }

            LocalizationManager.Instance.LoadLanguage(lang);
        }
    }
}