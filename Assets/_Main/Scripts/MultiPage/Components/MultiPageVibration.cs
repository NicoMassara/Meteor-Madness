using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.MultiPage.Components
{
#if UNITY_ANDROID 
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
#endif
}