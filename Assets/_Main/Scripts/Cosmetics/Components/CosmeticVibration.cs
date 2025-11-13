using System;
using _Main.Scripts.Cosmetics.MVC;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Cosmetics.Components
{
    public class CosmeticVibration : VibrationBehavior<CosmeticUIView>
    {
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnMainMenuButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
        }
    }
}