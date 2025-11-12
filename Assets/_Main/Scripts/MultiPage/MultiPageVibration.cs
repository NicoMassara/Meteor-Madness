using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.MultiPage
{
    public class MultiPageVibration : VibrationBehavior<MultiPageViewUI>
    {
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnNextButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnPreviousButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
        }
    }
}