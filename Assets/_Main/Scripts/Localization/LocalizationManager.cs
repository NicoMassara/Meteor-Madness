using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace _Main.Scripts.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance =>  _instance != null ? _instance : (_instance = CreateInstance());
        private static LocalizationManager _instance;
        
        private readonly SystemLanguage _defaultLanguage = GameParameters.GameplayValues.DefaultLanguage;
        private Dictionary<string, string> _localizedTexts = new();
        private SystemLanguage _currentLanguage;
        
        private Dictionary<SystemLanguage, string> _languageCodeMap = new Dictionary<SystemLanguage, string>
        {
            { SystemLanguage.English, "en" },
            { SystemLanguage.Spanish, "es" },
            { SystemLanguage.French, "fr" },
            { SystemLanguage.Portuguese, "pt" },
            { SystemLanguage.Italian, "it" },
            { SystemLanguage.German, "de" },
        };

        private readonly Dictionary<string, string> _textReplacement = new()
        {
            {"LeftKey", "A"},
            {"RightKey", "D"},
            {"AbilityKey", "S"}
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
            LoadLanguage(Application.systemLanguage);
        }

#if UNITY_ANDROID
        private IEnumerator LoadMobileJson(string path, SystemLanguage language)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"Language file for {language} not found. Falling back to default: {_defaultLanguage}");
                    string fallbackPath = Path.Combine(Application.streamingAssetsPath, "Localization", $"{_defaultLanguage.ToString().ToLower()}.json");
                    using (UnityWebRequest fallback = UnityWebRequest.Get(fallbackPath))
                    {
                        yield return fallback.SendWebRequest();
                        if (fallback.result == UnityWebRequest.Result.Success)
                            ParseJsonToDictionary(fallback.downloadHandler.text);
                        else
                            Debug.LogError("Failed to load fallback language file.");
                    }
                }
                else
                {
                    ParseJsonToDictionary(www.downloadHandler.text);
                }

                PlayerPrefs.SetString("language", language.ToString());
                PlayerPrefs.Save();

                LocalizationEvents.TriggerOnLanguageChanged();
            }
        }
#endif
        

        public void LoadLanguage(SystemLanguage language)
        {
            _currentLanguage = language;
            string langCode = GetLanguageCode(_currentLanguage).ToLower();
            string path = Path.Combine(Application.streamingAssetsPath, "Localization", $"{langCode}.json");

#if UNITY_ANDROID
            StartCoroutine(LoadMobileJson(path, language));
#else
            if (!File.Exists(path))
            {
                Debug.LogWarning(
                    $"Language file for {language} not found. Falling back to default: {_defaultLanguage}");
                path = Path.Combine(Application.streamingAssetsPath, "Localization",
                    $"{_defaultLanguage.ToString().ToLower()}.json");
            }

            string json = File.ReadAllText(path);
            ParseJsonToDictionary(json);

            PlayerPrefs.SetString("language", language.ToString());
            PlayerPrefs.Save();

            LocalizationEvents.TriggerOnLanguageChanged();
#endif
        }

        private void ParseJsonToDictionary(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("ParseJsonToDictionary recibió un JSON vacío o nulo.");
                return;
            }

            json = json.Trim('\uFEFF', '\u200B'); // limpia BOM o caracteres invisibles

            try
            {
                _localizedTexts.Clear();
                JObject root = JObject.Parse(json);
                FlattenJson(root, "");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parseando JSON: {e.Message}\nContenido:\n{json}");
            }
            
            LocalizationEvents.TriggerOnLocalizationLoaded();
#if !UNITY_ANDROID && !UNITY_IOS
            ReplacePlaceholderText();
#endif
        }
        
        private void FlattenJson(JToken token, string prefix)
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    string key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                    FlattenJson(property.Value, key);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    FlattenJson(array[i], $"{prefix}[{i}]");
                }
            }
            else
            {
                string value = token.ToString();

                // Soporte de @archivo
                if (value.StartsWith("@"))
                {
                    string relativePath = value.Substring(1);
                    string fullPath = Path.Combine(Application.streamingAssetsPath, "Localization", relativePath);

                    if (File.Exists(fullPath))
                    {
                        string fileContent = File.ReadAllText(fullPath, Encoding.UTF8);

                        // Si es JSON, parsear y aplanar recursivamente
                        if (fullPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                JToken externalJson = JToken.Parse(fileContent);

                                // Evitar duplicar prefijos si el JSON externo tiene el mismo key raíz
                                if (externalJson is JObject extObj && extObj.Properties().Count() == 1)
                                {
                                    FlattenJson(externalJson.First, prefix);
                                }
                                else
                                {
                                    FlattenJson(externalJson, prefix);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"Error parsing JSON file {fullPath}: {ex}");
                            }
                            return;
                        }
                        else // txt
                        {
                            value = fileContent;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"File not found: {fullPath}");
                    }
                }

                // Guardar el valor final
                _localizedTexts[prefix] = value;
            }
        }

        public string GetText(string key)
        {
            if (_localizedTexts.TryGetValue(key, out string value))
                return value;
            return $"[MISSING:{key}]";
        }

        public void ReplacePlaceholderText()
        {
            LocalizationTools.ReplacePlaceHolders(_localizedTexts, _textReplacement);
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