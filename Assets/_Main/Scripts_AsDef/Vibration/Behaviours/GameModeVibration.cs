using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;

namespace MeteorMadness.Vibration.Behaviours
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