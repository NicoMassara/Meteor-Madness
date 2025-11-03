using System;
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
                SoundEventCaller.PlayUIButton(UISoundType.Back);
            });
            GetUiComponents().NextButton.onClick.AddListener(() =>
            {
                OnNextButtonPressed?.Invoke();
                SoundEventCaller.PlayUIButton(UISoundType.Default);
            });
        }

        public void SetNextButtonText(string text)
        {
            GetUiComponents().NextButtonText.text = text;
        }

        public void SetEnablePreviousButton(bool isEnable)
        {
            GetUiComponents().PreviousButton.gameObject.SetActive(isEnable);
        }

        public void SetPanelText(string text)
        {
            GetUiComponents().PanelText.text = text;
        }

        public string GetNextButtonText()
        {
            return GetUiComponents().NextButtonText.text;
        }

        public void SetActiveMainPanel(bool isActive)
        {
            GetUiComponents().MainPanel.SetActive(isActive);
        }
    }
}