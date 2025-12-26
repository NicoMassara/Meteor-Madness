using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;

namespace MeteorMadness.Vibration.Behaviours
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