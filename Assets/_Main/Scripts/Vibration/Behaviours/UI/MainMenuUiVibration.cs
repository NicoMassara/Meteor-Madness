using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;

namespace MeteorMadness.Vibration.Behaviours
{
    public class MainMenuUiVibration : VibrationBehavior<IMainMenuUiVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnConfirmButtonClicked += () =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnCancelButtonClicked += () =>
            {
                Vibrate(UIVibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnBackButtonClicked += () =>
            {
                Vibrate(UIVibrationType.UIButtonCancel);
            };
        }
#endif
    }
}