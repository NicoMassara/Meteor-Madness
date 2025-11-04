using _Main.Scripts.Managers.UpdateManager;
using TMPro;
using UnityEngine;

namespace _Main.Scripts.Localization
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
            _text.text = LocalizationManager.Instance.GetText(textKey);
        }

        private void OnDestroy()
        {
            LocalizationEvents.OnLanguageChanged -= UpdateText;
        }
    }
}