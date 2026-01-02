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
                Vibrate(VibrationType.UIButtonAccept);
            };
            ComponentToVibrate.OnOptionsButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            ComponentToVibrate.OnMainMenuButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
        }
#endif
    }
}