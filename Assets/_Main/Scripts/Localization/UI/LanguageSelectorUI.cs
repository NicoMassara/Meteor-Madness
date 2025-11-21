using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MySettings;
using _Main.Scripts.MyTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Localization.UI
{
    public class LanguageSelectorUI : MonoBehaviour, ILanguageSelector
    {
        [Header("UI Components")] 
        [SerializeField] private Button languageButton;
        [SerializeField] private TMP_Text languageText;
        
        private int _currentLanguage;
        private int _languageCount;
        
        public event Action<int> OnChanged;

        private void Start()
        {
            _languageCount = LocalizationTools.LanguageCount;
        }

        private void OnEnable()
        {
            _currentLanguage = SettingsManager.Instance.GetLanguageIndex();
            UpdateValues(_currentLanguage);
            
            languageButton.onClick.AddListener(Button_OnClickHandler);
        }

        private void OnDisable()
        {
            languageButton.onClick.RemoveListener(Button_OnClickHandler);
        }

        private void Button_OnClickHandler()
        {
            _currentLanguage = ValueCarousel.GetValue(_currentLanguage,_languageCount);
            UpdateValues(_currentLanguage);
            OnChanged?.Invoke(_currentLanguage);
        }
        
        private void UpdateValues(int currentLanguage)
        {
            languageText.text = LocalizationTools.GetLocalizatedLanguage(currentLanguage);
        }
    }
}