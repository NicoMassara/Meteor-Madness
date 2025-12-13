using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public LanguageTextLoader(MonoBehaviour behaviour, Action onTextLoaded)
        {
            _behaviour = behaviour;
            _onTextLoaded += onTextLoaded;
            _onTextLoaded += OnTextLoadedHandler;
        }

        #region Public API

        public void SelectLanguageToLoad(SystemLanguage language)
        {
            _loadedText.Clear();
            
            string langCode = LocalizationTools.GetLanguageCode(language).ToLower();
            string path = Path.Combine(Application.streamingAssetsPath, "Localization", $"{langCode}.json");

            Debug.Log($"Path To Load: {path}");
            _behaviour.StartCoroutine(LoadAndProcessJson(path));
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

        private IEnumerator LoadAndProcessJson(string path)
        {
            JObject rootJson = null;
            yield return _behaviour.StartCoroutine(LoadJsonCoroutine(path, result => rootJson = result));

            if (rootJson == null)
            {
                Debug.LogError("Error cargando JSON: " + path);
                yield break;
            }

            FlattenJson("", rootJson);
            ReplacePlaceholderText();
            _onTextLoaded?.Invoke();
        }

        private IEnumerator LoadJsonCoroutine(string path, Action<JObject> callback)
        {
            string content = null;
            yield return _behaviour.StartCoroutine(ReadFileMobileCoroutine(path, result => content = result));
            content = content?.Trim() ?? "";
            
            if (!string.IsNullOrEmpty(content) && content[0] == '\uFEFF')
            {
                content = content.Substring(1);
            }

            if (string.IsNullOrEmpty(content))
            {
                callback?.Invoke(new JObject());
                yield break;
            }

            // Si es .txt
            if (path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                callback?.Invoke(new JObject { ["value"] = content });
                yield break;
            }

            JObject json = JObject.Parse(content);

            // Procesar recursivamente objetos internos
            foreach (var prop in json.Properties())
            {
                if (prop.Value.Type == JTokenType.Object)
                {
                    JObject nested = null;
                    yield return _behaviour.StartCoroutine(LoadJsonObjectCoroutine((JObject)prop.Value, Path.GetDirectoryName(path) ?? "", result => nested = result));
                    json[prop.Name] = nested;
                }
            }

            callback?.Invoke(json);
        }

        private IEnumerator LoadJsonObjectCoroutine(JObject obj, string currentDir, Action<JObject> callback)
        {
            foreach (var prop in obj.Properties())
            {
                if (prop.Value.Type == JTokenType.String && prop.Value.ToString().StartsWith("@"))
                {
                    string refPath = prop.Value.ToString().Substring(1);
                    string resolvedPath = Path.Combine(currentDir, refPath);

                    JObject loadedRef = null;
                    yield return _behaviour.StartCoroutine(LoadJsonCoroutine(resolvedPath, result => loadedRef = result));

                    obj[prop.Name] = loadedRef.ContainsKey("value") ? loadedRef["value"] : loadedRef;
                }
                else if (prop.Value.Type == JTokenType.Object)
                {
                    JObject nested = null;
                    yield return _behaviour.StartCoroutine(LoadJsonObjectCoroutine((JObject)prop.Value, currentDir, result => nested = result));
                    obj[prop.Name] = nested;
                }
            }

            callback?.Invoke(obj);
        }

        private IEnumerator ReadFileMobileCoroutine(string path, Action<string> callback)
        {
#if UNITY_ANDROID
            using (var www = UnityWebRequest.Get(path))
            {
                var op = www.SendWebRequest();
                while (!op.isDone)
                    yield return null;

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error leyendo archivo: " + path + " | " + www.error);
                    callback?.Invoke("");
                    yield break;
                }

                callback?.Invoke(www.downloadHandler.text);
            }
#else
            if (!File.Exists(path))
            {
                Debug.LogError("Archivo no encontrado: " + path);
                callback?.Invoke("");
            }
            else
            {
                callback?.Invoke(File.ReadAllText(path));
            }

            yield return null;
#endif
        }

        private void FlattenArray(string prefix, JArray array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                string key = $"{prefix}[{i}]";
                JToken value = array[i];

                switch (value.Type)
                {
                    case JTokenType.Object:
                        FlattenJson(key, (JObject)value);
                        break;
                    case JTokenType.Array:
                        FlattenArray(key, (JArray)value);
                        break;
                    default:
                        _loadedText[key] = value.ToString();
                        break;
                }
            }
        }

        private void FlattenJson(string prefix, JObject obj)
        {
            foreach (var prop in obj.Properties())
            {
                string key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";

                switch (prop.Value.Type)
                {
                    case JTokenType.Object:
                        FlattenJson(key, (JObject)prop.Value);
                        break;
                    case JTokenType.Array:
                        FlattenArray(key, (JArray)prop.Value);
                        break;
                    default:
                        _loadedText[key] = prop.Value.ToString();
                        break;
                }
            }
        }

        private void OnTextLoadedHandler()
        {
            // opcional
        }

        private void ReplacePlaceholderText()
        {
            LocalizationTools.ReplacePlaceHolders(_loadedText, _textReplacement);
        }

        #endregion
    }
}
