using _Main.Scripts.Interfaces.Vibration;
using UnityEngine;

namespace _Main.Scripts.Vibration.Behaviours.UI
{
    public class MainMenuUiVibration : VibrationBehavior<IMainMenuUiVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
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
#endif
    }
}