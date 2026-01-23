using MeteorMadness.Contracts.Events;
using NicolasMassara.CustomUpdateManager;
using TMPro;
using UnityEngine;

namespace MeteorMadness.Managers.Localization
{
    public class LocalizatedText : ManagedBehavior
    {
        [SerializeField] private string textKey;
        [SerializeField] private string prefix;
        [SerializeField] private string suffix;
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            UpdateText();
            LocalizationEvents.OnLanguageChanged += UpdateText;
        }
        
        private void UpdateText()
        {
            if(string.IsNullOrEmpty(textKey)) return;
            
            var localizedText = LocalizationManager.Instance.GetText(textKey);
            
            _text.text = $"{prefix}{localizedText}{suffix}";
        }

        private void OnDestroy()
        {
            LocalizationEvents.OnLanguageChanged -= UpdateText;
        }
    }
}