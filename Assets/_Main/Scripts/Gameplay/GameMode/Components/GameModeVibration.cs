using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeVibration : VibrationBehavior<GameModeView>
    {
        private void Start()
        {
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
}