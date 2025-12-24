using _Main.Scripts.Interfaces.Vibration;
using UnityEngine;

namespace _Main.Scripts.Vibration.Behaviours.UI
{
    public class CosmeticUiVibration : VibrationBehavior<ICosmeticUIVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnMainMenuButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
            ComponentToVibrate.OnSkinSelected += (value) =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };

            ComponentToVibrate.OnScroll += () =>
            {
                //Vibrate(VibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnCoinsFinishedDecrement += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnUnlockFailed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnUnlocked += () =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
        }
#endif
    }
}