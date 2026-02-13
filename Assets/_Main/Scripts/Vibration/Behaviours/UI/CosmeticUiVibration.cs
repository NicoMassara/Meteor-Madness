
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
                Vibrate(UIVibrationType.UIButtonCancel);
            };
            ComponentToVibrate.OnSkinSelected += (value) =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };

            ComponentToVibrate.OnScroll += (value) =>
            {
                if (value == 1)
                {
                    Vibrate(UIVibrationType.UIButtonAccept);
                }
                else
                {
                    Vibrate(UIVibrationType.UIButtonCancel);
                }
            };
            
            ComponentToVibrate.OnCoinsFinishedDecrement += () =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnUnlockFailed += () =>
            {
                Vibrate(UIVibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnUnlocked += () =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
        }
#endif
    }
}