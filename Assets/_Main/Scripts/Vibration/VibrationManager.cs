using System;
using _Main.Scripts.Contracts.Events;
using MeteorMadness.Contracts;
using MeteorMadness.GlobalValues.BaseSingleton;
using MeteorMadness.Managers;
using MeteorMadness.Vibration.So;
using UnityEngine;

namespace MeteorMadness.Vibration
{
#if UNITY_ANDROID
    public class VibrationManager : SingletonBehaviour<VibrationManager>
    {
        private VibrationController _vibrationController;
        private bool _canVibrate;

        private VibrationPriority _currentPriority;
        
        public event Action OnVibrate;
        public event Action OnStopVibration;

        public bool IsVibrating => _vibrationController.IsVibrating;

        private void Awake()
        {
            _vibrationController = new VibrationController();
        }

        private void Start()
        {
            
#if UNITY_EDITOR
            _canVibrate = GameParameters.DevelopmentValues.VibrationEnabled;
#else
            _canVibrate = SettingsManager.Instance.GetVibration();
#endif
            
            SettingsManager.Instance.OnVibrationChanged += (value) =>
            {
#if UNITY_EDITOR
                SetVibration(true);
#else
                SetVibration(value);
#endif

            };

            _vibrationController.OnVibrate += OnVibrate;
            _vibrationController.OnStopVibration += OnStopVibrationHandler;
        }

        private void OnStopVibrationHandler()
        {
            _currentPriority = VibrationPriority.None;
            OnStopVibration?.Invoke();
        }

        public void Vibrate(IVibrationData data)
        {
            if(_canVibrate == false) return;
            
            switch (data.VibrationApi)
            {
                case VibrationApiType.None:
                    return;
                case VibrationApiType.OneShot:
                    Vibrate(data.Data[0], data.Priority);
                    return;
            }

            if (data.Priority < _currentPriority) return;
            
            if (_currentPriority != VibrationPriority.None)
            {
                CancelVibration();
            }

            _currentPriority = data.Priority;
            
            var values = VibrationTools.CreateWaveformData(data.Data);
            _vibrationController.Vibrate(values.Durations, values.Intensities);
        }

        public void Vibrate(VibrationData data, VibrationPriority priority = VibrationPriority.Medium)
        {
            if(_canVibrate == false) return;
            
            if (priority < _currentPriority) return;
            
            if (_currentPriority != VibrationPriority.None)
            {
                CancelVibration();
            }
            
            _currentPriority = priority;
            
            _vibrationController.Vibrate(data.Duration, data.Intensity);
            VibrationEvents.TriggerOnVibrate(data.Duration, data.Intensity);
#if UNITY_EDITOR
            Debug.Log($"Vibrating : Duration - {data.Duration}, Intensity - {data.Intensity}");
#endif
        }

        public void Vibrate(DurationType duration, IntensityType intensity, VibrationPriority priority = VibrationPriority.Medium)
        {
            if(_canVibrate == false) return;
            if (priority < _currentPriority) return;
            
            if (_currentPriority != VibrationPriority.None)
            {
                CancelVibration();
            }
            
            _currentPriority = priority;
            
            _vibrationController.Vibrate(VibrationTools.GetDuration(duration), VibrationTools.GetIntensity(intensity));
            VibrationEvents.TriggerOnVibrate(VibrationTools.GetDuration(duration), VibrationTools.GetIntensity(intensity));
            
#if UNITY_EDITOR
            Debug.Log($"Vibrating : Duration - {duration}, Intensity - {intensity}");
#endif
        }

        public void Vibrate(UIVibrationType type)
        {
            if(_canVibrate == false) return;
            
            if(_currentPriority < VibrationPriority.Low) return;
            _currentPriority = VibrationPriority.Low;
            
            Vibrate(VibrationTools.GetUIVibration(type));
        }

        public void CancelVibration()
        {
#if UNITY_EDITOR
            Debug.Log("Vibration cancelled");
#endif
            _currentPriority = VibrationPriority.None;
            _vibrationController.CancelVibration();
            VibrationEvents.TriggerOnCancel();
        }

        private void SetVibration(bool canVibrate)
        {
            _canVibrate = canVibrate;
        }
    }
#endif
}