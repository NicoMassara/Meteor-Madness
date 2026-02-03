using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using MeteorMadness.Vibration;

namespace MeteorMadness.Vibration.Behaviours
{
    public class PauseUiVibration : VibrationBehavior<IPauseUIVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnResumeButtonPressed += () =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
            ComponentToVibrate.OnOptionsButtonPressed += () =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
            ComponentToVibrate.OnMainMenuButtonPressed += () =>
            {
                Vibrate(UIVibrationType.UIButtonCancel);
            };
        }
#endif
    }
}