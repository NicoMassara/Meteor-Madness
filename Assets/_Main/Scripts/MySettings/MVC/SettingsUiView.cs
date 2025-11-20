using System;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.MySettings.MVC
{
    public class SettingsUiView : MonoBehaviour, IObserver
    {
        [SerializeField] private SettingsUiSelector uiSelector;
        private SettingsUiComponents _uiComponents;
        
        public event Action<float> OnVolumeChanged;
        public event Action<int> OnLanguageChanged;
        public event Action<bool> OnVibrationChanged;
        
        public event Action OnBackButtonPressed;

        private void Start()
        {
            GetUiComponents().BackButton.onClick.AddListener(() => OnBackButtonPressed?.Invoke());
            /*GetUiComponents().VolumeSlider.OnChanged += OnVolumeChanged;
            GetUiComponents().VibrationToggle.OnChanged += OnVibrationChanged;
            GetUiComponents().LanguageSelector.OnChanged += OnLanguageChanged;*/
        }

        public void OnNotify(ulong message, params object[] args)
        {
            switch (message)
            {
                case SettingsObserverMessage.Enable:
                    HandleEnable();
                    break;
                case SettingsObserverMessage.Disable:
                    HandleDisable();
                    break;
            }
        }

        private void HandleEnable()
        {
            GetUiComponents().MainPanel.SetActive(true);
        }

        private void HandleDisable()
        {
            GetUiComponents().MainPanel.SetActive(false);
        }
        
        private SettingsUiComponents GetUiComponents()
        {
            return _uiComponents ??= uiSelector.GetPanelData();
        }
    }
}