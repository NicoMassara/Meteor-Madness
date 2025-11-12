using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageVibration : VibrationBehavior<MultiPageViewUI>
    {
        private void Start()
        {
            ComponentToVibrate.OnNextButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnNextButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
        }
    }
}