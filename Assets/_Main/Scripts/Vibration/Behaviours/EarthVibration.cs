using MeteorMadness.Contracts.Interfaces.Vibration;
using MeteorMadness.Vibration.BaseBehaviours;
using MeteorMadness.Vibration.So;
using UnityEngine;

namespace MeteorMadness.Vibration.Behaviours
{
    public class EarthVibration : VibrationBehavior<IEarthVibration>
    {
        [SerializeField] private VibrationDataSo collisionData;
        [SerializeField] private VibrationDataSo preDeath;
        [SerializeField] private VibrationDataSo death;
        [SerializeField] private VibrationDataSo reconstruction;
        
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
                Vibrate(preDeath);
            };
            
            ComponentToVibrate.OnVibrateDestruction += () =>
            {
                StopVibration();
                Vibrate(death);
            };
            
            ComponentToVibrate.OnReconstruct += () =>
            {
                Vibrate(reconstruction);
            };
        }
#endif
    }
}