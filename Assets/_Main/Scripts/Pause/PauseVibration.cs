using _Main.Scripts.Vibration;

namespace _Main.Scripts.Pause
{
    public class PauseVibration : VibrationBehavior<PauseUIView>
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