using System;
using MeteorMadness.GlobalValues.Interfaces;
using MeteorMadness.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace MeteorMadness.ScreenFlow.Settings
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