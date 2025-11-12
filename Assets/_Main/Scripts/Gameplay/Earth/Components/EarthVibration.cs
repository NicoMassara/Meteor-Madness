using System;
using _Main.Scripts.ScriptableObjects;
using _Main.Scripts.Vibration;
using UnityEngine;

namespace _Main.Scripts.Gameplay.Earth
{
    public class EarthVibration : VibrationBehavior<EarthView>
    {
        [SerializeField] private VibrationDataSo collisionData;
        
        private void Start()
        {
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
    }
}