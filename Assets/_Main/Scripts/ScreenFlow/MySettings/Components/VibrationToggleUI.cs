using System;
using MeteorMadness.GlobalValues.Events;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Managers.Localization;
using MeteorMadness.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.Settings
{
    public class VibrationToggleUI : MonoBehaviour,  IVibrationToggle
    {
        [Header("Components")]
        [SerializeField] private Button toggleButton;
        [SerializeField] private TMP_Text toggleText;
        [Header("Colors")] 
        [SerializeField] private Color enableColor = Color.green;
        [SerializeField] private Color disableColor =  Color.red;
        
        private bool _isEnabled;
        
        public event Action<bool> OnChanged;
        
        private void OnEnable()
        {
            // Get Value from SettingsManager
            _isEnabled = SettingsManager.Instance.GetVibration();
            LocalizationEvents.OnLocalizationLoaded += Localization_OnLocalizationLoadedHandler;
            UpdateUI(_isEnabled);
            //
            toggleButton.onClick.AddListener(Button_OnClickHandler);
        }

        private void OnDisable()
        {
            toggleButton.onClick.RemoveListener(Button_OnClickHandler);
            LocalizationEvents.OnLocalizationLoaded -= Localization_OnLocalizationLoadedHandler;
        }

        private void UpdateUI(bool isEnabled)
        {
            UpdateText(isEnabled);
            toggleButton.image.color = isEnabled ? enableColor : disableColor;
        }

        private void UpdateText(bool isEnabled)
        {
            var toggleString = isEnabled ? GetToggleText("Enable") : GetToggleText("Disable");
            toggleText.text = toggleString;
        }
        
        private string GetToggleText(string key)
        {
            return LocalizationManager.Instance.GetText($"UIButton.{key}");
        }
        
        private void Button_OnClickHandler()
        {
            _isEnabled = !_isEnabled;
            UpdateUI(_isEnabled);
            OnChanged?.Invoke(_isEnabled);
        }
        private void Localization_OnLocalizationLoadedHandler()
        {
            UpdateText(_isEnabled);
        }
    }
}