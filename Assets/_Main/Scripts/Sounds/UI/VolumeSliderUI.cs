using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.MySettings;
using UnityEngine;
using UnityEngine.UI;

namespace _Main.Scripts.Sounds.UI
{
    public class VolumeSliderUI : MonoBehaviour, IVolumeSlider
    {
        [SerializeField] private Slider volumeSlider;
        
        public event Action<float> OnChanged;

        private void Awake()
        {
            volumeSlider.wholeNumbers = true;
            volumeSlider.maxValue = 10;
            volumeSlider.minValue = 0;
        }

        private void OnEnable()
        {
            volumeSlider.value = SettingsManager.Instance.GetMasterVolume() * volumeSlider.maxValue;
            //
            volumeSlider.onValueChanged.AddListener(Slider_OnValueChangedHandler);
        }

        private void OnDisable()
        {
            volumeSlider.onValueChanged.RemoveListener(Slider_OnValueChangedHandler);
            //
        }

        private void Slider_OnValueChangedHandler(float value)
        {
            OnChanged?.Invoke(value/volumeSlider.maxValue);
        }
    }
}