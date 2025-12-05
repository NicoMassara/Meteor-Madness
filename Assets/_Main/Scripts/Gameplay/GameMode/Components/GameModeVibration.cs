using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeVibration : VibrationBehavior<GameModeView>
    {
#if UNITY_ANDROID 
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnCountDownFinished += () =>
            {
                Vibrate(VibrationDurationType.MediumLong,VibrationIntensityType.MediumHeavy);
            };
            
            ComponentToVibrate.OnCountdownUpdated += () =>
            {
                Vibrate(VibrationDurationType.Short,VibrationIntensityType.MediumLight);
            };
        }
#endif
    }
}