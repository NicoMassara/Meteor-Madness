using System;
using MeteorMadness.Contracts.Interfaces;
using MeteorMadness.GlobalValues.Tools;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.Settings
{
    public class LanguageSelectorUI : MonoBehaviour, ILanguageSelector
    {
        [Header("UI Components")] 
        [SerializeField] private Button languageButton;
        [SerializeField] private TMP_Text languageText;
        
        private int _currentLanguage;
        private int _languageCount;
        
        public event Action<int> OnChanged;

        private void Awake()
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