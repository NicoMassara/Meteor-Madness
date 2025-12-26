using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;

namespace MeteorMadness.Vibration.Behaviours
{
    public class DefeatUIVibration : VibrationBehavior<IDefeatUIVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnMainMenuButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
            ComponentToVibrate.OnRestartButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
        }
#endif
    }
}