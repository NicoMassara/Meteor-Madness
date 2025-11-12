using System;
using _Main.Scripts.Interfaces;
using _Main.Scripts.Vibration;
using UnityEngine;

namespace _Main.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_VibrationData_Name", menuName = "Scriptable Objects/Vibration/Data", order = 0)]
    public class VibrationDataSo : ScriptableObject, IVibrationData
    {
        [SerializeField] private VibrationData data;
        [SerializeField] private VibrationDurationType duration;
        [SerializeField] private VibrationIntensityType intensity;
        [SerializeField] private VibrationType type;

        public VibrationData Data => data;

        private void OnValidate()
        {
            UpdateData();
        }

        private void UpdateData()
        {
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
            }
        }
    }
}