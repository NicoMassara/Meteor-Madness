using _Main.Scripts.Interfaces.Vibration;

namespace _Main.Scripts.Vibration.Behaviours
{
    public class GameModeVibration : VibrationBehavior<IGameModeVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnCountDownFinished += () =>
            {

            };
            
            ComponentToVibrate.OnCountdownUpdated += (time) =>
            {
                if (time > 1)
                {
                    Vibrate(VibrationDurationType.Short,VibrationIntensityType.MediumLight);
                }
                else if (time <= 0)
                {
                    Vibrate(VibrationDurationType.MediumLong,VibrationIntensityType.MediumHeavy);
                }
            };
        }
#endif
    }
}