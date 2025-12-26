using MeteorMadness.GlobalValues.Interfaces.Vibration;
using MeteorMadness.Vibration;
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
                    VibrationTools.GetIntensity(VibrationIntensityType.ExtraLight),
                    VibrationTools.GetIntensity(VibrationIntensityType.MediumHeavy),value);
                Vibrate(new VibrationData
                {
                    Intensity = vibrationForce,
                    Duration = VibrationTools.GetDuration(VibrationDurationType.Short),
                });
            };
            
            ComponentToVibrate.OnLanguageChanged += (value) =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.OnVibrationChanged += (value) =>
            {
                Vibrate(value ? VibrationType.UIButtonAccept : VibrationType.UIButtonCancel);
            };
            
            ComponentToVibrate.OnBackButtonPressed += () =>
            {
                Vibrate(VibrationType.UIButtonCancel);
            };
            
        }
#endif
    }
}