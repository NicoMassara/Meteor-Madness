using _Main.Scripts.Interfaces;
using _Main.Scripts.Vibration;
using UnityEngine;

namespace _Main.Scripts.Vibration
{
#if UNITY_ANDROID 
    [CreateAssetMenu(fileName = "SO_VibrationData_Name", menuName = "Scriptable Objects/Vibration/Data", order = 0)]
    public class VibrationDataSo : ScriptableObject, IVibrationData
    {
        [SerializeField] private VibrationData data;
        [SerializeField] private VibrationDurationType duration;
        [SerializeField] private VibrationIntensityType intensity;
        [SerializeField] private VibrationType type;

        private int _lastIntensity;
        private long _lastDuration;

        public VibrationData Data => data;

        private void OnValidate()
        {
            UpdateData();
        }

        private void UpdateData()
        {
            if (data.Duration != _lastDuration)
            {
                duration = VibrationDurationType.None;
                type = VibrationType.None;
            }
            
            if (data.Intensity != _lastIntensity)
            {
                intensity = VibrationIntensityType.None;
                type = VibrationType.None;
            }

            if (duration != VibrationDurationType.None)
            {
                data.Duration = VibrationTools.GetDuration(duration);
            }

            if (intensity != VibrationIntensityType.None)
            {
                data.Intensity = VibrationTools.GetIntensity(intensity);
            }

            if (type != VibrationType.None)
            {
                data = VibrationTools.GetType(type);
                duration = VibrationDurationType.None;
                intensity = VibrationIntensityType.None;
            }
            
            _lastIntensity = data.Intensity;
            _lastDuration = data.Duration;
        }
    }
#endif
}