using System;
using _Main.Scripts.Gameplay.GameMode;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Defeat
{
    public class DefeatVibration : VibrationBehavior<DefeatUIView>
    {
#if UNITY_ANDROID 
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
        }
#endif
    }
}