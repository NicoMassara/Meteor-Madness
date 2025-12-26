using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.Vibration.So;
using UnityEngine;

namespace MeteorMadness.Vibration.BaseBehaviours
{
    
    public abstract class VibrationBehavior<T> : MonoBehaviour
    where T : IVibrationComponent
    {
#if UNITY_ANDROID
        private VibrationManager _vibration;
        
        protected T ComponentToVibrate { get; private set; }
        
        protected bool IsVibrating => _vibration.IsVibrating;

        protected virtual void Awake()
        {
#if !UNITY_EDITOR
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                enabled = false;
            }
#endif
            
        }

        protected virtual void Start()
        {
            ComponentToVibrate = GetComponent<T>();
            _vibration = VibrationManager.Instance;
        }

        #region Vibration Actions

        protected virtual void Vibrate(VibrationDataSo soData)
        {
            if (soData == null)
            {
                Debug.LogWarning($"Vibration data is null in {gameObject.name}");
                return;
            }

            Vibrate(soData.Data);
        }

        public void Vibrate(VibrationData data)
        {
            if (data == null)
            {
                Debug.LogWarning($"Vibration data is null in {gameObject.name}");
                return;
            }
            
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

        #endregion
#endif
    }
}
