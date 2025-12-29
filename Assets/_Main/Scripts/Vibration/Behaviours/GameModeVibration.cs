using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using UnityEngine;

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
                Vibrate(VibrationDurationType.MediumLong,VibrationIntensityType.Heavy);
            };
            
            ComponentToVibrate.OnCountdownUpdated += (time) =>
            {
                Vibrate(VibrationDurationType.ExtraShort,VibrationIntensityType.MediumLight);
            };
        }
#endif
    }
}