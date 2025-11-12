using System;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Vibration
{
    public abstract class VibrationBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        private VibrationManager _vibration;
        protected T ComponentToVibrate { get; private set; }
        
        protected bool IsVibrating => _vibration.IsVibrating;

        protected virtual void Awake()
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                enabled = false;
            }
            
            _vibration = VibrationManager.Instance;
            ComponentToVibrate = GetComponent<T>();
        }

        public void Vibrate(VibrationDataSo soData)
        {
            Vibrate(soData.Data);
        }

        public void Vibrate(VibrationData data)
        {
            _vibration.Vibrate(data);
        }

        public void Vibrate(VibrationDurationType duration, VibrationIntensityType intensity)
        {
            _vibration.Vibrate(duration, intensity);
        }

        public void Vibrate(VibrationType type)
        {
            _vibration.Vibrate(type);
        }

        public void StopVibration()
        {
            _vibration.CancelVibration();
        }
    }
}