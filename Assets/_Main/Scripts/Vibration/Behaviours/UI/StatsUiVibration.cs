using _Main.Scripts.Interfaces.Vibration;

namespace _Main.Scripts.Vibration.Behaviours.UI
{
    public class StatsUiVibration : VibrationBehavior<IStatsUIVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnBackButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnCloseFirstButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
        }
#endif
    }
}