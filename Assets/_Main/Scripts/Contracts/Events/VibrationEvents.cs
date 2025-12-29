using System;

namespace _Main.Scripts.Contracts.Events
{
    public class VibrationEvents
    {
        /// <summary>
        /// Duration in ms || Intensity
        /// </summary>
        public static Action<long, int> OnVibrate;

        public static void TriggerOnVibrate(long duration = 100, int amplitude = -1) 
            => OnVibrate?.Invoke(duration, amplitude);
        
        // ========================================== //

        public static Action OnCancel;
        public static void TriggerOnCancel() => OnCancel?.Invoke();
    }
}