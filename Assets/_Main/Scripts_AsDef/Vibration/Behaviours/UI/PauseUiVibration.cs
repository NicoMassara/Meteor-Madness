using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;

namespace _Main.Scripts.Vibration.Behaviours.UI
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