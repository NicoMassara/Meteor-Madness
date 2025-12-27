
using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;

namespace MeteorMadness.Vibration.Behaviours
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

            ComponentToVibrate.OnScroll += (value) =>
            {
                if (value == 1)
                {
                    Vibrate(VibrationType.UIButtonAccept);
                }
                else
                {
                    Vibrate(VibrationType.UIButtonCancel);
                }
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