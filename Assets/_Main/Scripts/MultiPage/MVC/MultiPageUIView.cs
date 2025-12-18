using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Interfaces.Vibration;
using _Main.Scripts.Localization;
using NicolasMassara.CustomTimerManager;
using NicolasMassara.CustomUpdateManager;
using UnityEngine;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageViewUI : ManagedBehavior, IMultiPageUISounds, IMultiPageUIVibration
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
        
        private string GetLocalizedString(string key)
        {
            return LocalizationManager.Instance.GetText(key);
        }

        public void DisableNextButton()
        {
            GetUiComponents().NextButton.interactable = false;
            
            TimerManager.Add(new TimerData(1.25f, () =>
            {
                GetUiComponents().NextButton.interactable = true;
            }));
        }
    }
}