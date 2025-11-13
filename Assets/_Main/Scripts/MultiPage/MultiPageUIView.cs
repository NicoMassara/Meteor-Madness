using System;
using _Main.Scripts.Localization;
using _Main.Scripts.Managers.UpdateManager;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageViewUI : ManagedBehavior
    {
        [SerializeField] private MultiPageUiSelector uiSelector;

        private MultiPageUIComponents _uiComponents;
        public event Action OnPreviousButtonPressed;
        public event Action OnNextButtonPressed;

        private MultiPageUIComponents GetUiComponents()
        {
            return _uiComponents ??= _uiComponents = uiSelector.GetPanelData();
        }

        private void Awake()
        {
            GetUiComponents().PreviousButton.onClick.AddListener(() =>
            {
                OnPreviousButtonPressed?.Invoke();
            });
            GetUiComponents().NextButton.onClick.AddListener(() =>
            {
                OnNextButtonPressed?.Invoke();
            });
        }
        

        public void SetNextButtonText(string textCode)
        {
            GetUiComponents().NextButtonText.text = GetLocalizedString($"{textCode}");;
        }

        public void SetEnablePreviousButton(bool isEnable)
        {
            GetUiComponents().PreviousButton.gameObject.SetActive(isEnable);
        }

        public void SetPanelText(string textCode, int index)
        {
            GetUiComponents().PanelText.text = GetLocalizedString($"{textCode}[{index}]");
        }

        public void SetActiveMainPanel(bool isActive)
        {
            GetUiComponents().MainPanel.SetActive(isActive);
        }
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }
    }
}