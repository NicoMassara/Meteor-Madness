using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace _Main.Scripts.Localization
{
    public class LanguageTextLoader
    {
        private readonly Dictionary<string, string> _textReplacement = new()
        {
            {"LeftKey", "<color=blue>A</color>"},
            {"RightKey", "<color=blue>D</color>"},
            {"AbilityKey", "<color=blue>S</color>"}
        };
        
        private readonly Dictionary<string, string> _loadedText = new Dictionary<string, string>();
        private MonoBehaviour _behaviour;
        
        private event Action _onTextLoaded;

        public LanguageTextLoader(MonoBehaviour behaviour,Action onTextLoaded)
        {
            _behaviour = behaviour;
            _onTextLoaded += onTextLoaded;
            _onTextLoaded += OnTextLoadedHandler;
        }

        #region Public API
        
        public async Task SelectLanguageToLoad(SystemLanguage language)
        {
            _loadedText.Clear();
            
            string langCode = LocalizationTools.GetLanguageCode(language).ToLower();
            string path = Path.Combine(Application.streamingAssetsPath, "Localization", $"{langCode}.json");
            
            Debug.Log($"Path To Load: {path}");
            
            JObject rootJson = await LoadJsonRecursive(path);
            FlattenJson("", rootJson);
            
        }
        
        public int GetArrayLength(string prefix)
        {
            return _loadedText.Keys.Count(
                key => key.StartsWith($"{prefix}[") && key.Contains("]"));
        }
        
        public string GetText(string key)
        {
            return _loadedText.TryGetValue(key, out string value) ? value : $"[MISSING:{key}]";
        }
        
        #endregion
        
        #region Private API

        // Carga recursiva de JSON/TXT
        private async Task<JObject> LoadJsonRecursive(string path)
        {
            string content = await ReadFileMobile(path);
            content = content.Trim();

            // Si es .txt solo devolvemos un objeto simple
            if (path.EndsWith(".txt"))
            {
                return new JObject { ["value"] = content };
            }

            JObject json = JObject.Parse(content);

            foreach (var prop in json.Properties())
            {
                if (prop.Value.Type == JTokenType.String && prop.Value.ToString().StartsWith("@"))
                {
                    string refPath = prop.Value.ToString().Substring(1);
                    string resolvedPath = Path.Combine(Path.GetDirectoryName(path), refPath);
                    JObject loadedRef = await LoadJsonRecursive(resolvedPath);

                    if (loadedRef.ContainsKey("value"))
                        json[prop.Name] = loadedRef["value"];
                    else
                        json[prop.Name] = loadedRef;
                }
                else if (prop.Value.Type == JTokenType.Object)
                {
                    json[prop.Name] = await LoadJsonRecursiveObject(prop.Value as JObject, Path.GetDirectoryName(path));
                }
            }

            return json;
        }

        private async Task<JObject> LoadJsonRecursiveObject(JObject obj, string currentDir)
        {
            foreach (var prop in obj.Properties())
            {
                if (prop.Value.Type == JTokenType.String && prop.Value.ToString().StartsWith("@"))
                {
                    string refPath = prop.Value.ToString().Substring(1);
                    string resolvedPath = Path.Combine(currentDir, refPath);
                    JObject loadedRef = await LoadJsonRecursive(resolvedPath);

                    if (loadedRef.ContainsKey("value"))
                        obj[prop.Name] = loadedRef["value"];
                    else
                        obj[prop.Name] = loadedRef;
                }
                else if (prop.Value.Type == JTokenType.Object)
                {
                    obj[prop.Name] = await LoadJsonRecursiveObject(prop.Value as JObject, currentDir);
                }
            }
            return obj;
        }

        // Lectura de archivo compatible con mobile
        private async Task<string> ReadFileMobile(string path)
        {
            if (path.Contains("://") || path.Contains(":///")) // ya es URL
            {
                UnityWebRequest www = UnityWebRequest.Get(path);
                await www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error leyendo archivo: {path} | {www.error}");
                    return "";
                }
                return www.downloadHandler.text;
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                UnityWebRequest www = UnityWebRequest.Get("jar:file://" + path);
                await www.SendWebRequest();
                return www.downloadHandler.text;
            }
            else
            {
                return File.ReadAllText(path);
            }
        }

        // Convierte el JSON anidado a diccionario con claves tipo "en.settings.title"
        private void FlattenJson(string prefix, JObject obj)
        {
            foreach (var prop in obj.Properties())
            {
                string key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                if (prop.Value.Type == JTokenType.Object)
                    FlattenJson(key, (JObject)prop.Value);
                else
                    _loadedText[key] = prop.Value.ToString();
            }

            ReplacePlaceholderText();
            
            _onTextLoaded?.Invoke();
        }
        
        
        private void OnTextLoadedHandler()
        {
  
        }
        
        private void ReplacePlaceholderText()
        {
            LocalizationTools.ReplacePlaceHolders(_loadedText, _textReplacement);
        }
        
        #endregion
    }
}