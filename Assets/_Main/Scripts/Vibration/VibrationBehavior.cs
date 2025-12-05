using _Main.Scripts.MySettings;
using _Main.Scripts.ScriptableObjects;
using UnityEngine;

namespace _Main.Scripts.Vibration
{
    
    public abstract class VibrationBehavior<T> : MonoBehaviour
    {
#if UNITY_ANDROID 
        private VibrationManager _vibration;

        private bool _canVibrate;
        protected T ComponentToVibrate { get; private set; }
        
        protected bool IsVibrating => _vibration.IsVibrating;

        protected virtual void Awake()
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                enabled = false;
            }
            
            ComponentToVibrate = GetComponent<T>();
        }

        protected virtual void Start()
        {
            _vibration = VibrationManager.Instance;
            SettingsManager.Instance.OnVibrationChanged += Settings_OnVibrationChangedHandler;
            _canVibrate = SettingsManager.Instance.GetVibration();
        }

        private void OnDestroy()
        {
            SettingsManager.Instance.OnVibrationChanged -= Settings_OnVibrationChangedHandler;
        }

        #region Vibration Actions

        protected virtual void Vibrate(VibrationDataSo soData)
        {
            if(_canVibrate == false) return;
            
            Vibrate(soData.Data);
        }

        public void Vibrate(VibrationData data)
        {
            if(_canVibrate == false) return;
            
            _vibration.Vibrate(data);
        }

        public void Vibrate(VibrationDurationType duration, VibrationIntensityType intensity)
        {
            if(_canVibrate == false) return;
            
            _vibration.Vibrate(duration, intensity);
        }

        public void Vibrate(VibrationType type)
        {
            if(_canVibrate == false) return;
            
            _vibration.Vibrate(type);
        }

        public void StopVibration()
        {
            _vibration.CancelVibration();
        }

        #endregion
        
        
        private void Settings_OnVibrationChangedHandler(bool canVibrate)
        {
            _canVibrate = canVibrate;
        }
#endif
    }
}
