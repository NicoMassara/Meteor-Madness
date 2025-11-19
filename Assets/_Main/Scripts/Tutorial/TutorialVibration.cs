using _Main.Scripts.Tutorial.MVC;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Tutorial
{
    
#if UNITY_ANDROID 
    public class TutorialVibration : VibrationBehavior<TutorialUIView>
    {
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnStartButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
        }
    }
#endif
}