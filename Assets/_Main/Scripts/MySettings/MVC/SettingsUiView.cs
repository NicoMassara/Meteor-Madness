using System;
using _Main.Scripts.Interfaces.Sounds;
using _Main.Scripts.Observer;
using UnityEngine;

namespace _Main.Scripts.MySettings.MVC
{
    public class SettingsUiView : MonoBehaviour, IObserver, ISettingsUISounds
    {
        [SerializeField] private SettingsUiSelector uiSelector;
        private SettingsUiComponents _uiComponents;
        
#pragma warning disable CS0067 // Event is never used
        public event Action<float> OnVolumeChanged;
        public event Action<int> OnLanguageChanged;
        public event Action<bool> OnVibrationChanged;
        
#pragma warning restore CS0067 // Event is never used
        
        public event Action OnBackButtonPressed;
        

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
            //
            GetUiComponents().BackButton.onClick.AddListener(() => OnBackButtonPressed?.Invoke());
            GetUiComponents().VolumeSlider.OnChanged += OnVolumeChanged;
#if UNITY_ANDROID
            
            GetUiComponents().VibrationToggle.OnChanged += OnVibrationChanged;
#endif
            GetUiComponents().LanguageSelector.OnChanged += OnLanguageChanged;
        }

        private void HandleDisable()
        {
            GetUiComponents().BackButton.onClick.RemoveAllListeners();
            GetUiComponents().VolumeSlider.OnChanged -= OnVolumeChanged;
#if UNITY_ANDROID
            GetUiComponents().VibrationToggle.OnChanged -= OnVibrationChanged;
#endif
            GetUiComponents().LanguageSelector.OnChanged -= OnLanguageChanged;
            //
            GetUiComponents().MainPanel.SetActive(false);
        }
        
        private SettingsUiComponents GetUiComponents()
        {
            return _uiComponents ??= uiSelector.GetPanelData();
        }
    }
}