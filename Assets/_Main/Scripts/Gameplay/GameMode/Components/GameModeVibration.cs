using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Gameplay.GameMode
{
#if UNITY_ANDROID 
    public class GameModeVibration : VibrationBehavior<GameModeView>
    {
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnCountdownUpdatedFinished += () =>
            {
                Vibrate(VibrationDurationType.MediumLong,VibrationIntensityType.MediumHeavy);
            };
            
            ComponentToVibrate.OnCountdownUpdated += () =>
            {
                Vibrate(VibrationDurationType.Short,VibrationIntensityType.MediumLight);
            };
        }
    }
#endif
}