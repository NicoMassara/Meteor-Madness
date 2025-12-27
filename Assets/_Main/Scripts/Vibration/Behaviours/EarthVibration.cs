using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using MeteorMadness.Vibration;
using MeteorMadness.Vibration.So;
using UnityEngine;

namespace MeteorMadness.Vibration.Behaviours
{
    public class EarthVibration : VibrationBehavior<IEarthVibration>
    {
        [SerializeField] private VibrationDataSo collisionData;
#if UNITY_ANDROID
        
        protected override void Start()
        {
            base.Start();
            ComponentToVibrate.OnCollision += () =>
            {
                Vibrate(collisionData);
            };
            
            ComponentToVibrate.OnPreDestruction += () =>
            {
                Vibrate(new VibrationData
                {
                    Duration = 5000,
                    Intensity = VibrationTools.GetIntensity(VibrationIntensityType.ExtraHeavy)
                });
            };
            
            ComponentToVibrate.OnVibrateDestruction += () =>
            {
                StopVibration();
                Vibrate(VibrationDurationType.ExtraShort, VibrationIntensityType.MediumHeavy);
            };
            
            ComponentToVibrate.OnReconstruct += () =>
            {
                Vibrate(VibrationDurationType.ExtraShort, VibrationIntensityType.Heavy);
            };
        }
#endif
    }
}