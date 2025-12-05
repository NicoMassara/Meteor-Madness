using System;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Gameplay.GameMode
{
    public class GameModeUIVibration : VibrationBehavior<GameModeUIView>
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