using UnityEngine;

namespace MeteorMadness.Vibration.So
{
    public interface IVibrationData
    {
        public VibrationData[] Data { get; }
        public VibrationApiType VibrationApi { get; }
        public VibrationPriority Priority { get; }
    }

    [CreateAssetMenu(fileName = "SO_VibrationData_Default", menuName = "Scriptable Objects/Vibration/Data", order = 0)]
    public class VibrationDataSo : ScriptableObject, IVibrationData
    {
        [SerializeField] private VibrationData[] vibrationData;
        [SerializeField] private VibrationPriority priority;

        public VibrationData[] Data => vibrationData;
        public VibrationApiType VibrationApi => GetVibrationApiType();
        public VibrationPriority Priority => priority;

        private VibrationApiType GetVibrationApiType()
        {
            return Data.Length switch
            {
                1 => VibrationApiType.OneShot,
                > 2 => VibrationApiType.Waveform,
                _ => VibrationApiType.None
            };
        }
    }
}