using _Main.Scripts.Interfaces.Vibration;

namespace _Main.Scripts.Vibration.Behaviours.Animation
{
    public class DefeatAnimationVibration : VibrationBehavior<IDefeatAnimationVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnScoreMoved += () =>
            {
                Vibrate(new VibrationData
                {
                    Duration = VibrationTools.GetDuration(VibrationDurationType.Short),
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.Light)
                });
            };
            
            ComponentToVibrate.OnHighScoreMoved += () =>
            {
                Vibrate(new VibrationData
                {
                    Duration = VibrationTools.GetDuration(VibrationDurationType.Short),
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.Light)
                });
            };
            
            ComponentToVibrate.OnNewHighScore += () =>
            {
                Vibrate(new VibrationData
                {
                    Duration = VibrationTools.GetDuration(VibrationDurationType.Short),
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.MediumLight)
                });
            };
            
            ComponentToVibrate.OnCoinsMoved += () =>
            {
                Vibrate(new VibrationData
                {
                    Duration = VibrationTools.GetDuration(VibrationDurationType.Short),
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.Light)
                });
            };
            
            ComponentToVibrate.OnTitleMoved += () =>
            {
                Vibrate(new VibrationData
                {
                    Duration = VibrationTools.GetDuration(VibrationDurationType.Short),
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.Medium)
                });
            };
        }
#endif
    }
}