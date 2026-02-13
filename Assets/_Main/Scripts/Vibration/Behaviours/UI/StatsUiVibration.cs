using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using MeteorMadness.Vibration;

namespace MeteorMadness.Vibration.Behaviours
{
    public class StatsUiVibration : VibrationBehavior<IStatsUIVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnBackButtonPressed += () =>
            {
                Vibrate(UIVibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnCloseFirstButtonPressed += () =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
        }
#endif
    }
}