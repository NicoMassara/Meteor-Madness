using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;

namespace MeteorMadness.Vibration.Behaviours
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