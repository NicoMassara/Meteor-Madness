using System;
using _Main.Scripts.Cosmetics.MVC;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Cosmetics.Components
{
    public class CosmeticVibration : VibrationBehavior<CosmeticUIView>
    {
        private void Start()
        {
            ComponentToVibrate.OnMainMenuButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
        }
    }
}