using _Main.Scripts.Interfaces.Vibration;

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
        }
#endif
    }
}