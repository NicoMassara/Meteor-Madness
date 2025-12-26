using NicolasMassara.CustomUpdateManager;
using TMPro;
using MeteorMadness.GlobalValues.Events;
using UnityEngine;

namespace MeteorMadness.Managers.Localization
{
    public class LocalizatedText : ManagedBehavior
    {
        [SerializeField] private string textKey;
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
            
            _text.text = LocalizationManager.Instance.GetText(textKey);
        }

        private void OnDestroy()
        {
            LocalizationEvents.OnLanguageChanged -= UpdateText;
        }
    }
}