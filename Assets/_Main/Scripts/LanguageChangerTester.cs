using _Main.Scripts.Localization;
using UnityEngine;

namespace _Main.Scripts
{
    public class LanguageChangerTester : MonoBehaviour
    {
        [SerializeField] private Language language;

        private void OnValidate()
        {
            LanguageSelector.ChangeLanguage(language.ToString().ToLower());
        }
    }

    public enum Language
    {
        English,
        Spanish,
        French,
        Italian,
        German,
        Portuguese
    }
}