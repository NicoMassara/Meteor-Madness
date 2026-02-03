
using MeteorMadness.Contracts.Interfaces.Vibration;
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

        protected virtual void Vibrate(IVibrationData data)
        {
            if (data == null)
            {
                Debug.LogWarning($"Vibration data is null in {gameObject.name}");
                return;
            }

            _vibration.Vibrate(data);
        }

        public void Vibrate(VibrationData data, VibrationPriority priority)
        {
            if (data == null)
            {
                Debug.LogWarning($"Vibration data is null in {gameObject.name}");
                return;
            }
            
            _vibration.Vibrate(data, priority);
        }

        public void Vibrate(DurationType duration, IntensityType intensity, VibrationPriority priority)
        {
            _vibration.Vibrate(duration, intensity,priority);
        }

        public void Vibrate(UIVibrationType type)
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
