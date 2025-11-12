using System;
using _Main.Scripts.MyComponents;
using UnityEngine;

namespace _Main.Scripts.Vibration
{
    public class VibrationManager : SingletonBehaviour<VibrationManager>
    {
        private VibrationController _vibrationController;
        
        public event Action OnVibrate;
        public event Action OnStopVibration;

        private void Awake()
        {
            _vibrationController = new VibrationController();
        }

        private void Start()
        {
            _vibrationController.OnVibrate += OnVibrate;
            _vibrationController.OnStopVibration += OnStopVibration;
        }

        public void Vibrate(VibrationData data)
        {
            _vibrationController.Vibrate(data.Duration, data.Intensity);
        }

        public void Vibrate(VibrationDurationType duration, VibrationIntensityType intensity)
        {
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
    }

    [Serializable]
    public class VibrationData
    {
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

}