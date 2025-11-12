using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeVibration : VibrationBehavior<GameModeUIView>
    {
        private void Start()
        {
            ComponentToVibrate.OnMainMenuButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
            ComponentToVibrate.OnRestartButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            ComponentToVibrate.OnPauseButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnPointsAdded += () =>
            {
                Vibrate(new VibrationData
                {
                    Duration = 10,
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.Medium)
                });
            };
        }
    }
}