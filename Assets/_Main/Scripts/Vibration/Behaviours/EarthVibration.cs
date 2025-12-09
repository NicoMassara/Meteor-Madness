using _Main.Scripts.Interfaces.Vibration;
using UnityEngine;

namespace _Main.Scripts.Vibration.Behaviours
{
    public class EarthVibration : VibrationBehavior<IEarthVibration>
    {
#if UNITY_ANDROID 
        [SerializeField] private VibrationDataSo collisionData;
        
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
            
            ComponentToVibrate.OnDestruction += () =>
            {
                StopVibration();
                Vibrate(VibrationDurationType.ExtraShort, VibrationIntensityType.MediumHeavy);
            };
        }
#endif
    }
}