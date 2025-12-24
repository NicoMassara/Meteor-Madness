using _Main.Scripts.Interfaces.Vibration;

namespace _Main.Scripts.Vibration.Behaviours.UI
{
    public class MultiPageUiVibration : VibrationBehavior<IMultiPageUIVibration>
    {
#if UNITY_ANDROID
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
#endif
    }
}