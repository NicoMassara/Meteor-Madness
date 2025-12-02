using System.Collections;
using _Main.Scripts.MyComponents;
using UnityEngine;
using _Main.Scripts.MySettings;

namespace _Main.Scripts.Localization
{
    public class LocalizationManager : SingletonBehaviour<LocalizationManager>
    {
        private LanguageTextLoader _textLoader;
        
        private void Start()
        {
            _textLoader = new LanguageTextLoader(this,OnTextLoadedHandler);
            Initialize();
        }
        
        private void Initialize()
        {
            StartCoroutine(WaitForSettingsData());

            SettingsManager.Instance.OnLanguageChanged += Settings_OnLanguageChangedHandler;
        }

        private IEnumerator WaitForSettingsData()
        {
            var settings = SettingsManager.Instance;
            
            float timeout = 5f;
            float timer = 0f;
            
            yield return new WaitUntil(() =>
            {
                timer += Time.deltaTime;
                return settings.HasLoadedData() || timer >= timeout;
            });
            
            if (!settings.HasLoadedData())
            {
                Debug.LogWarning("Data could not be loaded. Please check your settings file.");
            }

            var languageIndex = settings.GetLanguageIndex();

            // If language is not set, it will get the System Language.
            if (languageIndex == -1)
            {
                Debug.LogWarning("Language not selected, loading system language.");
                // If system language is not compatible, it'll return English
                
                languageIndex = LocalizationTools.GetIndexFromLanguage(Application.systemLanguage);
                
                // And then saves it 
                settings.SetLanguageIndex(languageIndex);
                settings.SaveSettings();
            }
            
            LoadLanguage(LocalizationTools.GetLanguageFromIndex(languageIndex));

            yield return null;
        }

        #region Public API
        
        public void LoadLanguage(SystemLanguage language)
        {
            _textLoader?.SelectLanguageToLoad(language);
        }

        public int GetArrayLength(string prefix)
        {
            return _textLoader.GetArrayLength(prefix);
        }
        
        public string GetText(string key)
        {
            return _textLoader.GetText(key);
        }

        #endregion
        
        #region Handlers
        
        private void Settings_OnLanguageChangedHandler(int languageIndex)
        {
            _textLoader?.SelectLanguageToLoad(LocalizationTools.GetLanguageFromIndex(languageIndex));
        }
        
        private void OnTextLoadedHandler()
        {
            LocalizationEvents.TriggerOnLocalizationLoaded();
            LocalizationEvents.TriggerOnLanguageChanged();
        }
        
        #endregion
    }
}