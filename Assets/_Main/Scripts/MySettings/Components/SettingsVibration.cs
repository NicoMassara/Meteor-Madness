using _Main.Scripts.MySettings.UI;
using _Main.Scripts.Vibration;
using UnityEngine;

namespace _Main.Scripts.MySettings.Components
{
    public class SettingsVibration : VibrationBehavior<SettingsUIView>
    {
#if UNITY_ANDROID 
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.VolumeSlider.OnChanged += (value) =>
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
            
            ComponentToVibrate.LanguageSelector.OnChanged += (value) =>
            {
                Vibrate(VibrationType.UIButtonAccept);
            };
            
            ComponentToVibrate.VibrationToggle.OnChanged += (value) =>
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