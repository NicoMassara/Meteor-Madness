using System;
using _Main.Scripts.MyComponents;
using _Main.Scripts.MySettings;
using UnityEngine;

namespace _Main.Scripts.Vibration
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

    [Serializable]
    public class VibrationData
    {
        [Tooltip("In ms")]
        [Range(10,3000)]
        public long Duration;
        [Range(1,255)]
        public int Intensity;
    }
    
    public enum VibrationDurationType
    {
        None,
        ExtraShort,
        Short,
        MediumShort,
        Medium,
        MediumLong,
        Long,
        ExtraLong,
        SuperLong
    }

    public enum VibrationIntensityType
    {
        None,
        ExtraLight,
        Light,
        MediumLight,
        Medium,
        MediumHeavy,
        Heavy,
        ExtraHeavy,
        FullHard
    }
    
    public enum VibrationType
    {
        None,
        UIButtonAccept,
        UIButtonCancel
    }
#endif
}