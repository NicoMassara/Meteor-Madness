using System;
using _Main.Scripts.MySettings.UI;
using UnityEngine;

namespace _Main.Scripts.MySettings
{
    [RequireComponent(typeof(SettingsUIView))]
    public class SettingsController : MonoBehaviour
    {
        private SettingsUIView _ui;

        private void Awake()
        {
            _ui = GetComponent<SettingsUIView>();
        }

        private void Start()
        {
            _ui.VolumeSlider.OnChanged += (value) =>
            {
                SettingsManager.Instance.SetMasterVolume(value);
            };

#if UNITY_ANDROID && !UNITY_EDITOR

            _ui.VibrationToggle.OnChanged += (value) =>
            {
                SettingsManager.Instance.SetVibration(value);
            };
#endif

            
            _ui.LanguageSelector.OnChanged += (value) =>
            {
                SettingsManager.Instance.SetLanguageIndex(value);
            };

            _ui.OnBackButtonPressed += () =>
            {
                SettingsManager.Instance.SaveSettings();
            };
        }
    }
}