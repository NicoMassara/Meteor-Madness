using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Gameplay.GameMode
{
#if UNITY_ANDROID 
    public class GameModeUIVibration : VibrationBehavior<GameModeUIView>
    {
        protected override void Start()
        {
            base.Start();
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
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.Light)
                });
            };
        }
    }
#endif
}