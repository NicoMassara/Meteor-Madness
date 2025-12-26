using System;
using MeteorMadness.GlobalValues.BaseSingleton;
using MeteorMadness.Managers;
using UnityEngine;

namespace MeteorMadness.Vibration
{
#if UNITY_ANDROID
    public class VibrationManager : SingletonBehaviour<VibrationManager>
    {
        private VibrationController _vibrationController;

        private bool _canVibrate;
        
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
            _canVibrate = true;
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
            _vibrationController.OnStopVibration += OnStopVibration;
        }

        public void Vibrate(VibrationData data)
        {
            if(_canVibrate == false) return;
            
            _vibrationController.Vibrate(data.Duration, data.Intensity);
#if UNITY_EDITOR
            Debug.Log($"Vibrating : Duration - {data.Duration}, Intensity - {data.Intensity}");
#endif
        }

        public void Vibrate(VibrationDurationType duration, VibrationIntensityType intensity)
        {
            if(_canVibrate == false) return;
            
            _vibrationController.Vibrate(VibrationTools.GetDuration(duration), VibrationTools.GetIntensity(intensity));
            
#if UNITY_EDITOR
            Debug.Log($"Vibrating : Duration - {duration}, Intensity - {intensity}");
#endif
        }

        public void Vibrate(VibrationType type)
        {
            Vibrate(VibrationTools.GetType(type));
        }

        public void CancelVibration()
        {
#if UNITY_EDITOR
            Debug.Log("Vibration cancelled");
#endif
            _vibrationController.CancelVibration();
        }

        private void SetVibration(bool canVibrate)
        {
            _canVibrate = canVibrate;
        }
    }
#endif
}