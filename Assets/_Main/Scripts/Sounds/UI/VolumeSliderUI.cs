using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MySettings;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Sounds.UI
{
    public class VolumeSliderUI : MonoBehaviour, IVolumeSlider
    {
        private Slider _volumeSlider;
        
        public event Action<float> OnChanged;

        private void Awake()
        {
            _volumeSlider = GetComponent<Slider>();
            
            _volumeSlider.wholeNumbers = true;
            _volumeSlider.maxValue = 10;
            _volumeSlider.minValue = 0;
        }

        private void OnEnable()
        {
            _volumeSlider.value = SettingsManager.Instance.GetMasterVolume() * _volumeSlider.maxValue;
            //
            _volumeSlider.onValueChanged.AddListener(Slider_OnValueChangedHandler);
        }

        private void OnDisable()
        {
            _volumeSlider.onValueChanged.RemoveListener(Slider_OnValueChangedHandler);
            //
        }

        private void Slider_OnValueChangedHandler(float value)
        {
            OnChanged?.Invoke(value/_volumeSlider.maxValue);
        }
    }
}