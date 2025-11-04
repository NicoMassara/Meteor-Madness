using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace _Main.Scripts.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static LocalizationManager _instance;
        
        private SystemLanguage _defaultLanguage = SystemLanguage.English;
        private Dictionary<string, string> _localizedTexts = new();
        private SystemLanguage _currentLanguage;
        
        private Dictionary<SystemLanguage, string> _languageCodeMap = new Dictionary<SystemLanguage, string>
        {
            { SystemLanguage.English, "en" },
            { SystemLanguage.Spanish, "es" },
            { SystemLanguage.French, "fr" },
        };
        
        private void Awake()
        {
            Initialize();
        }
        
        private static LocalizationManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(LocalizationManager))
            {
                hideFlags = HideFlags.DontSave,
            };
            DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<LocalizationManager>();
        }

        private void Initialize()
        {
            string savedLang = PlayerPrefs.GetString("language", "");
            SystemLanguage lang = string.IsNullOrEmpty(savedLang)
                ? Application.systemLanguage
                : (SystemLanguage)Enum.Parse(typeof(SystemLanguage), savedLang);
            
            LoadLanguage(lang);
        }

        public void LoadLanguage(SystemLanguage language)
        {
            _currentLanguage = language;
            string path = Path.Combine(Application.streamingAssetsPath, "Localization", $"{GetLanguageCode(_currentLanguage).ToLower()}.json");

            if (!File.Exists(path))
            {
                Debug.LogWarning($"Language file for {language} not found. Falling back to default: {_defaultLanguage}");
                path = Path.Combine(Application.streamingAssetsPath, "Localization", $"{_defaultLanguage.ToString().ToLower()}.json");
            }
            
            string json = File.ReadAllText(path);
            ParseJsonToDictionary(json);
            
            PlayerPrefs.SetString("language", language.ToString());
            PlayerPrefs.Save();
            
            LocalizationEvents.TriggerOnLanguageChanged();
        }
        
        private void ParseJsonToDictionary(string json)
        {
            _localizedTexts.Clear();

            JObject root = JObject.Parse(json);
            FlattenJson(root, "");
        }
        
        private void FlattenJson(JObject obj, string prefix)
        {
            foreach (var property in obj.Properties())
            {
                string key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

                if (property.Value is JObject nested)
                {
                    FlattenJson(nested, key);
                }
                else
                {
                    _localizedTexts[key] = property.Value.ToString();
                }
            }
        }

        public string GetText(string key)
        {
            if (_localizedTexts.TryGetValue(key, out string value))
                return value;
            return $"[MISSING:{key}]";
        }

        public SystemLanguage GetCurrentLanguage() => _currentLanguage;

        private string GetLanguageCode(SystemLanguage language)
        {
            if (_languageCodeMap.TryGetValue(language, out var code))
            {
                return code;
            }
            
            Debug.LogWarning("Language Could not Be Found in CodeMap, returning default.");
            return _languageCodeMap[SystemLanguage.English];
        }

    }
}