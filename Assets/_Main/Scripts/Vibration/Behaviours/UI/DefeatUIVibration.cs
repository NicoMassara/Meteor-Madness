using MeteorMadness.Contracts.Interfaces.Vibration;
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
                Vibrate(UIVibrationType.UIButtonCancel);
            };
            ComponentToVibrate.OnRestartButtonPressed += () =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
        }
#endif
    }
}