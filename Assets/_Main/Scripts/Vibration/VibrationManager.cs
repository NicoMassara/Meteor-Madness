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
            _canVibrate = SettingsManager.Instance.GetVibration();
            
            SettingsManager.Instance.OnVibrationChanged += (value) =>
            {
                SetVibration(value);
            };

            _vibrationController.OnVibrate += OnVibrate;
            _vibrationController.OnStopVibration += OnStopVibration;
        }

        public void Vibrate(VibrationData data)
        {
            if(_canVibrate == false) return;
            
            _vibrationController.Vibrate(data.Duration, data.Intensity);
        }

        public void Vibrate(VibrationDurationType duration, VibrationIntensityType intensity)
        {
            if(_canVibrate == false) return;
            
            _vibrationController.Vibrate(VibrationTools.GetDuration(duration), VibrationTools.GetIntensity(intensity));
        }

        public void Vibrate(VibrationType type)
        {
            Vibrate(VibrationTools.GetType(type));
        }

        public void CancelVibration()
        {
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