using _Main.Scripts.Interfaces.Vibration;

namespace _Main.Scripts.Vibration.Behaviours.UI
{
    public class GameModeUIVibration : VibrationBehavior<IGameModeUIVibration>
    {
#if UNITY_ANDROID 
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnPauseButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnPointsAdded += () =>
            {
                Vibrate(new VibrationData
                {
                    Duration = 10,
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.Light)
                });
            };
        }
#endif
    }
}