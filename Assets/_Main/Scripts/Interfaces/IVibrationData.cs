using _Main.Scripts.Vibration;

namespace _Main.Scripts.Interfaces
{
#if UNITY_ANDROID 
    public interface IVibrationData
    {
        public VibrationData Data { get; }
    }
#endif
}