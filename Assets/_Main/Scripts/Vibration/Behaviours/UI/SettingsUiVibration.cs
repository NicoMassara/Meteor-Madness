using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using UnityEngine;

namespace MeteorMadness.Vibration.Behaviours
{
    public class SettingsUiVibration : VibrationBehavior<ISettingsUiVibration>
    {
#if UNITY_ANDROID
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnVolumeChanged += (value) =>
            {
                
                var vibrationForce = (int)Mathf.Lerp(
                    VibrationTools.GetIntensity(IntensityType.ExtraLight),
                    VibrationTools.GetIntensity(IntensityType.MediumHeavy),value);
                Vibrate(new VibrationData
                {
                    Intensity = vibrationForce,
                    Duration = VibrationTools.GetDuration(DurationType.Short),
                }, VibrationPriority.Low);
            };
            
            ComponentToVibrate.OnLanguageChanged += (value) =>
            {
                Vibrate(UIVibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnVibrationChanged += (value) =>
            {
                Vibrate(value ? UIVibrationType.UIButtonAccept : UIVibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnBackButtonPressed += () =>
            {
                Vibrate(UIVibrationType.UIButtonCancel);
            };
            
        }
#endif
    }
}