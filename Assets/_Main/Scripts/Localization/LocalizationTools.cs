using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace _Main.Scripts.Localization
{
    public class LocalizationTools
    {
        private static readonly Regex _placeholderRegex = new Regex(@"%%(.*?)%%");
        public const int LanguageCount = 6;


        public static void ReplacePlaceHolders(
            Dictionary<string, string> localizatedText, 
            Dictionary<string, string> replacementText)
        {
            var keys = new List<string>(localizatedText.Keys);
            
            foreach (var key in keys)
            {
                string original = localizatedText[key];
                string processed = _placeholderRegex.Replace(original, match =>
                {
                    string placeholderKey = match.Groups[1].Value;
                    if (replacementText.TryGetValue(placeholderKey, out string value))
                        return value;

                    return match.Value; // si no hay reemplazo, deja el texto como está
                });

                localizatedText[key] = processed;
            }
        }
        
        
        public static string GetLanguageCode(SystemLanguage language)
        {
            return language switch
            {
                SystemLanguage.English => "en",
                SystemLanguage.Spanish => "es",
                SystemLanguage.French => "fr",
                SystemLanguage.Portuguese => "pt",
                SystemLanguage.Italian => "it",
                SystemLanguage.German => "de",
                _ => "en"
            };
        }
        
        public static int GetIndexFromLanguage(SystemLanguage language)
        {
            return language switch
            {
                SystemLanguage.English => 0,
                SystemLanguage.Spanish => 1,
                SystemLanguage.French => 2,
                SystemLanguage.Portuguese => 3,
                SystemLanguage.Italian => 4,
                SystemLanguage.German => 5,
                _ => 0
            };
        }

        public static SystemLanguage GetLanguageFromIndex(int index)
        {
            return index switch
            {
                0 => SystemLanguage.English,
                1 => SystemLanguage.Spanish,
                2 => SystemLanguage.French,
                3 => SystemLanguage.Portuguese,
                4 => SystemLanguage.Italian,
                5 => SystemLanguage.German,
                _ => SystemLanguage.English
            };
        }

        public static string GetLocalizatedLanguage(int index)
        {
            return index switch
            {
                0 => "English",
                1 => "Español",
                2 => "Français",
                3 => "Português",
                4 => "Italiano",
                5 => "Deutsch",
                _ => "English"
            };
        }
        
    }   
}   