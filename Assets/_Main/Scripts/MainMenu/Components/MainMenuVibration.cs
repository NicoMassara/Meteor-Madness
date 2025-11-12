using System;
using _Main.Scripts.MainMenu.MVC;
using _Main.Scripts.Vibration;

namespace _Main.Scripts.Menu
{
    public class MainMenuVibration : VibrationBehavior<MainMenuUiView>
    {
        private void Start()
        {
            ComponentToVibrate.OnConfirmButtonClicked += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnCancelButtonClicked += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnBackButtonClicked += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
        }
    }
}